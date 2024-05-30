using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using App.Contracts.BLL;
using App.DTO.v1_0;
using App.DTO.v1_0.Identity;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;
using AppUser = App.Domain.Identity.AppUser;

namespace WebApp.ApiControllers;

/// <summary>
/// Authentication controller
/// </summary>
[ApiVersion( "1.0" )]
[ApiController]
[Route("api/v{version:apiVersion}/Authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private const int ExpiresInSeconds = 3600;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="bll"></param>
    /// <param name="usermanager"></param>
    /// <param name="configuration"></param>
    public AuthenticationController(IAppBLL bll, UserManager<AppUser> usermanager, IConfiguration configuration)
    {
        _bll = bll;
        _userManager = usermanager;
        _configuration = configuration;
    }

    /// <summary>
    /// Log in method
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Jwt string it manages to log in</returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("Login")]
    [ProducesResponseType<string>((int)HttpStatusCode.OK)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<JwtResponse>> AuthoriseUser([FromBody] LoginInfo model)
    {
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return BadRequest(new RestApiErrorResponse {Error = "Password or Email field is empty", Status = HttpStatusCode.BadRequest});
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return NotFound(new RestApiErrorResponse {Error = "Email unknown", Status = HttpStatusCode.NotFound});
        
        var isAuthenticated = await _userManager.CheckPasswordAsync(user, model.Password);
        
        if (isAuthenticated)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email!)
            };
            
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            
            var refreshToken = await JwtTokenHelper.GiveNewRefreshToken(_bll, user, 1);
            var response = new JwtResponse()
            {
                Jwt = JwtTokenHelper.GenerateJwt(claims, _configuration, ExpiresInSeconds),
                RefreshToken = refreshToken,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = await _userManager.GetRolesAsync(user),
                Email = model.Email,
                UserId = user.Id.ToString()
            };
            return Ok(response);
        }
        else
        {
            return Unauthorized(new RestApiErrorResponse { Error = "Wrong password or email" , Status = HttpStatusCode.Unauthorized});
        }
    }


    /// <summary>
    /// This method lets you register new account
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Jwt token after account was created</returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("Register")]
    [ProducesResponseType<string>((int)HttpStatusCode.OK)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.Conflict)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<JwtResponse>> RegisterUser([FromBody] RegisterInfo model)
    {
        if (string.IsNullOrEmpty(model.Email) || 
            string.IsNullOrEmpty(model.Password) ||
            string.IsNullOrEmpty(model.Firstname) || 
            string.IsNullOrEmpty(model.Lastname))
        {
            return BadRequest("Email and password are required.");
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return Conflict(new RestApiErrorResponse { Error = "Email is already in use.", Status = HttpStatusCode.Conflict});
        }

        var newUser = new AppUser { UserName = model.Email, Email = model.Email, FirstName = model.Firstname, LastName = model.Lastname};

        var result = await _userManager.CreateAsync(newUser, model.Password);
        if (result.Succeeded)
        {
            return await AuthoriseUser(new LoginInfo { Email = model.Email, Password = model.Password });
        }
        else
        {
            return BadRequest(new RestApiErrorResponse() {Error = "User creation failed", Status = HttpStatusCode.BadRequest});
        }
    }

    
    /// <summary>
    /// This method lets user to refresh their jwt
    /// </summary>
    /// <param name="model"></param>
    /// <returns>New Jwt with their refresh token</returns>
    [HttpPatch]
    [Route("RefreshJwt")]
    [ProducesResponseType<JwtResponse>((int)HttpStatusCode.OK)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.NotFound)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.Unauthorized)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<TokenRefreshInfo>> RefreshJwt([FromBody] TokenRefreshInfo model)
{
    try
    {
        string[] parts = model.Jwt.Split(".");
        var payload = parts[1];
        payload = payload.PadRight(payload.Length + (payload.Length * 3) % 4, '=');
        byte[] payloadBytes = Convert.FromBase64String(payload);
        string decodedPayload = Encoding.UTF8.GetString(payloadBytes);
        
        var jwtPayload = JwtPayload.Deserialize(decodedPayload);
        
        if (!jwtPayload.Keys.Contains("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"))
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound });
        }
    
        var userEmail = jwtPayload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"].ToString();
        
        var user = await _userManager.FindByEmailAsync(userEmail!);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse { Error = "User does not exist", Status = HttpStatusCode.NotFound });
        }
        
        if (!await _bll.AppRefreshTokens.isValid(model.RefreshToken))
        {
            return BadRequest(new RestApiErrorResponse { Error = "Refresh token is not valid", Status = HttpStatusCode.BadRequest });
        }
        
        if (!await JwtTokenHelper.CheckJwtExpired(model.Jwt, _configuration))
        {
            return Unauthorized(new RestApiErrorResponse { Error = "JWT is invalid", Status = HttpStatusCode.Unauthorized });
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email!)
        };
        
        if (roles.Any())
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var updatedResponse = new TokenRefreshInfo()
        {
            Jwt = JwtTokenHelper.GenerateJwt(claims, _configuration, ExpiresInSeconds),
            RefreshToken = model.RefreshToken,
        };

        return Ok(updatedResponse);
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred during JWT refresh: " + ex.Message);
        return StatusCode(StatusCodes.Status500InternalServerError, new RestApiErrorResponse { Error = "Internal server error", Status = HttpStatusCode.InternalServerError });
    }
}


    /// <summary>
    /// This method will set refresh token expirery date
    /// </summary>
    /// <param name="model"></param>
    /// <returns>That it did it</returns>
    [HttpPatch]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("LogOut")]
    [ProducesResponseType<JwtResponse>((int)HttpStatusCode.OK)]
    [ProducesResponseType<RestApiErrorResponse>((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<bool>> LogOut([FromBody] LogoutInfo model)
    {
        var result = await JwtTokenHelper.ExpireRefreshToken(model.RefreshToken, _bll);
        if (!result) return NotFound(new RestApiErrorResponse {Error = "Token couldn't be expired", Status = HttpStatusCode.NotFound});
        return Ok(result);
    }
}
