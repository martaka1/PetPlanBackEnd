using System.Net;
using System.Security.Claims;
using App.BLL.DTO.HelperEnums;
using App.Contracts.BLL;
using App.Domain.Identity;
using App.DTO.v1_0;
using Asp.Versioning;
using AutoMapper;
using DAL.App.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Helpers;
using EExcerciseLevel = App.DTO.v1_0.HelperEnums.EExcerciseLevel;
using EExcerciseType = App.DTO.v1_0.HelperEnums.EExcerciseType;


namespace WebApp.ApiControllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ExcerciseController :ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.Excercise, App.BLL.DTO.Excercise> _mapper;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="bll"></param>
    /// <param name="userManager"></param>
    /// <param name="autoMapper"></param>
    public ExcerciseController(AppDbContext context, IAppBLL bll, UserManager<AppUser> userManager,
        IMapper autoMapper)
    {
        _context = context;
        _bll = bll;
        _userManager = userManager;
        _mapper = new PublicDTOBllMapper<App.DTO.v1_0.Excercise, App.BLL.DTO.Excercise>(autoMapper);
    }

    /// <summary>
    /// Get all excercises
    /// </summary>
    /// <returns>List of excercises</returns>
    [HttpGet]
    [Route("AllExcercises")]
    [ProducesResponseType<IEnumerable<App.DTO.v1_0.Excercise>>((int) HttpStatusCode.OK)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<App.DTO.v1_0.Excercise>>> GetExcercise()
    {
        var res = (await _bll.Excercise.GetAllExcercisesIncludingAsync())
            .Select(e => _mapper.Map(e))
            .ToList();
        return Ok(res);
    }

    /// <summary>
    /// Get spesific excercise
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Excercise or not found</returns>
    [HttpGet]
    [Route("GetExcercise/{id}")]
    [ProducesResponseType<App.DTO.v1_0.Excercise>((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<App.DTO.v1_0.Excercise>> GetExcercise(Guid id)
    {
        var excercise = await _bll.Excercise.FirstOrDefaultAsync(id);

        if (excercise == null)
        {
            return NotFound();
        }

        return Ok(excercise);
    }

    /// <summary>
    /// Update excercise
    /// </summary>
    /// <param name="excercise"></param>
    /// <returns>Updated excercise</returns>
    [HttpPatch]
    [Route("UpdateExcercie")]
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> PutExcercise( App.DTO.v1_0.Excercise excercise)
    {
        var userEmalClaim =
            HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmalClaim == null)
        {
            return NotFound(new RestApiErrorResponse
                { Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound });
        }

        var userEmail = userEmalClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse
                { Error = "User does not excist", Status = HttpStatusCode.NotFound });
        }

        try
        {
            var excisting = await _bll.Excercise.FirstOrDefaultAsync(excercise.Id);
            var updatedExcercise = new App.BLL.DTO.Excercise()
            {
                AppUserId = excisting!.AppUserId,
                Description = excercise.Description,
                Id = excisting.Id,
                Name = excercise.Name
            };
            switch (excercise.Type)
            {
                case EExcerciseType.Alone:
                    updatedExcercise.Type = App.BLL.DTO.HelperEnums.EExcerciseType.Alone;
                    break;
                case EExcerciseType.Inside:
                    updatedExcercise.Type = App.BLL.DTO.HelperEnums.EExcerciseType.Inside;
                    break;
                case EExcerciseType.Other:
                    updatedExcercise.Type = App.BLL.DTO.HelperEnums.EExcerciseType.Other;
                    break;
                case EExcerciseType.Outside:
                    updatedExcercise.Type = App.BLL.DTO.HelperEnums.EExcerciseType.Outside;
                    break;
                case EExcerciseType.InGroups:
                    updatedExcercise.Type = App.BLL.DTO.HelperEnums.EExcerciseType.InGroups;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (excercise.Level)
            {
                case EExcerciseLevel.Advanced:
                    updatedExcercise.Level = App.BLL.DTO.HelperEnums.EExcerciseLevel.Advanced;
                    break;
                case EExcerciseLevel.Beginner:
                    updatedExcercise.Level = App.BLL.DTO.HelperEnums.EExcerciseLevel.Beginner;
                    break;
                case EExcerciseLevel.Medium:
                    updatedExcercise.Level = App.BLL.DTO.HelperEnums.EExcerciseLevel.Medium;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var found = await _bll.Excercise.FirstOrDefaultAsync(updatedExcercise.Id);

            if (found == null) return NotFound(new RestApiErrorResponse() { Error = $"item with id: {excercise.Id} was not found", Status = HttpStatusCode.NotFound});;
            
            _bll.Excercise.Update(updatedExcercise);
            await _bll.SaveChangesAsync();
            return Ok(excercise);

        }
        catch (Exception )
        {
            return StatusCode(500); // Handle exceptions appropriately
        }
    }


    /// <summary>
    /// Add new excercie
    /// </summary>
    /// <param name="excercise"></param>
    /// <returns>Added excercise</returns>
    [HttpPost]
    [Route("AddExcercise")]
    [ProducesResponseType<Excercise>((int) HttpStatusCode.Created)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<App.DTO.v1_0.Excercise>> PostExcercise(App.DTO.v1_0.Excercise excercise)
    {
        var userEmalClaim =
            HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmalClaim == null)
        {
            return NotFound(new RestApiErrorResponse
                { Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound });
        }

        var userEmail = userEmalClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse
                { Error = "User does not excist", Status = HttpStatusCode.NotFound });
        }
        if (string.IsNullOrEmpty(excercise.Name) ||
            string.IsNullOrEmpty(excercise.Description) )
        {
            return BadRequest(new RestApiErrorResponse() {Error = "One or more fields is empty", Status = HttpStatusCode.BadRequest});
        }
        try
        {
            var newExcercise = new App.BLL.DTO.Excercise()
            {
                AppUserId = user.Id,
                Description = excercise.Description,
                Name = excercise.Name,
            };
            switch (excercise.Type)
            {
                case EExcerciseType.Alone:
                    newExcercise.Type = App.BLL.DTO.HelperEnums.EExcerciseType.Alone;
                    break;
                case EExcerciseType.Inside:
                    newExcercise.Type = App.BLL.DTO.HelperEnums.EExcerciseType.Inside;
                    break;
                case EExcerciseType.Other:
                    newExcercise.Type = App.BLL.DTO.HelperEnums.EExcerciseType.Other;
                    break;
                case EExcerciseType.Outside:
                    newExcercise.Type = App.BLL.DTO.HelperEnums.EExcerciseType.Outside;
                    break;
                case EExcerciseType.InGroups:
                    newExcercise.Type = App.BLL.DTO.HelperEnums.EExcerciseType.InGroups;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (excercise.Level)
            {
                case EExcerciseLevel.Advanced:
                    newExcercise.Level = App.BLL.DTO.HelperEnums.EExcerciseLevel.Advanced;
                    break;
                case EExcerciseLevel.Beginner:
                    newExcercise.Level = App.BLL.DTO.HelperEnums.EExcerciseLevel.Beginner;
                    break;
                case EExcerciseLevel.Medium:
                    newExcercise.Level = App.BLL.DTO.HelperEnums.EExcerciseLevel.Medium;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var added = _bll.Excercise.Add(newExcercise);
            await _bll.SaveChangesAsync();
            return Ok(_mapper.Map(added));

        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });

        }
    }

    /// <summary>
    /// Delete excercise
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true or false</returns>
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [HttpDelete]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("DeleteExcercise/{id}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> DeleteExcercise(Guid id)
    {
        var excercise = await _bll.Excercise.FirstOrDefaultAsync(id);
        if (excercise == null)
        {
            return NotFound();
        }
        var userEmalClaim =
            HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmalClaim == null)
        {
            return NotFound(new RestApiErrorResponse
                { Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound });
        }

        var userEmail = userEmalClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse
                { Error = "User does not excist", Status = HttpStatusCode.NotFound });
        }
        var rolesClaim = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        if (rolesClaim.Count == 0)
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not contain any role claims", Status = HttpStatusCode.NotFound });
        }

        bool isAdmin = rolesClaim.Any(claim => claim.Value == "Admin");
        Console.WriteLine(isAdmin);
        try
        {
            if (user.Id == excercise.AppUserId || isAdmin)
            {
                await _bll.Excercise.RemoveAsync(excercise);
                await _bll.SaveChangesAsync();
                return NoContent();
            }

            else
            {
                return BadRequest(new RestApiErrorResponse()
                    
                    { Error = "Cannot delete someone elses excercise", Status = HttpStatusCode.BadRequest});
            }
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });
        }
    }
}