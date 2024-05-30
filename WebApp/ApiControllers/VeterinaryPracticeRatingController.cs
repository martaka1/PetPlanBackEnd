using System.Net;
using System.Security.Claims;
using App.Contracts.BLL;
using App.Domain.Identity;
using App.DTO.v1_0;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;

namespace WebApp.ApiControllers;



/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/VetrinaryPracticeRating")]
public class VeterinaryPracticeRatingController : ControllerBase
{
    
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.VeterinaryPracticeRating, App.BLL.DTO.VeterinaryPracticeRating> _mapper;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bll"></param>
    /// <param name="userManager"></param>
    /// <param name="mapper"></param>
    public VeterinaryPracticeRatingController(IAppBLL bll, UserManager<AppUser> userManager, IMapper mapper)
    {
        _bll = bll;
        _userManager = userManager;
        _mapper = new PublicDTOBllMapper<VeterinaryPracticeRating, App.BLL.DTO.VeterinaryPracticeRating>(mapper);
    }
    
    
    /// <summary>
    /// Returns all veterinary practice ratings from the database
    /// </summary>
    /// <returns>list of veterinary practice ratings</returns>
    [HttpGet]
    [Route("AllVeterinaryPracticeRating")]
    [ProducesResponseType(typeof(IEnumerable<VeterinaryPracticeRating>), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<IEnumerable<VeterinaryPracticeRating>>> GetAllVeterinaryPracticeRatings()
    {
        var found = (await _bll.VeterinaryPracticeRating.GetAllVeterinaryPracticeRatingsIncludingAsync())
            .Select(rating => _mapper.Map(rating));
      
        return Ok(found);
    }
    
    
    
    /// <summary>
    /// Returns specific veterinary practice rating from the database
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Specific veterinarypractice rating or not found</returns>
    [HttpGet]
    [Route("GetVeterinaryPracticeRatingRating/{id}")]
    [ProducesResponseType(typeof(VeterinaryPracticeRating), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<VeterinaryPracticeRating>> GetVeterinaryPracticeRating(Guid id)
    {
        if (id == default) return BadRequest(id);
        var found = (await _bll.VeterinaryPracticeRating.GetAllVeterinaryPracticeRatingsIncludingAsync()).FirstOrDefault(kb => kb.Id == id);

        if (found == null) return NotFound(new RestApiErrorResponse {Error = $"Rating with id: {id} was not found", Status = HttpStatusCode.NotFound});
        return Ok(_mapper.Map(found));
    }
    
    
        /// <summary>
    /// Returns speicific user VeterinaryPracticeRating 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>List of VeterinaryPracticeRating </returns>
    [HttpGet]
    [Route("GetUserVeterinaryPracticeRating/")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(VeterinaryPracticeRating), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<IEnumerable<VeterinaryPracticeRating>>> GetUserVeterinaryPracticeRatings()
    {
        var userEmailClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmailClaim == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound});
        }
    
        var userEmail = userEmailClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "User does not exist that was inside JWT", Status = HttpStatusCode.NotFound});
        }
        
        var found = (await _bll.VeterinaryPracticeRating.GetAllVeterinaryPracticeRatingsIncludingAsync()).Where(bp => bp.UserId == user.Id);
        
        return Ok(found);
    }
        
    
    /// <summary>
    /// Delete VeterinaryPracticeRating from the database
    /// </summary>
    /// <returns>true or false depending on if it did its job</returns>
    [HttpDelete]
    [Route("DeleteVeterinaryPracticeRating/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<VeterinaryPracticeRating>> DeleteVeterinaryPracticeRating(Guid id)
    {
        var userEmailClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmailClaim == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound});
        }
    
        var userEmail = userEmailClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "User does not exist that was inside JWT", Status = HttpStatusCode.NotFound});
        }
        
        var rolesClaim = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        if (rolesClaim.Count == 0)
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not contain any role claims", Status = HttpStatusCode.NotFound });
        }
        
        var found = await _bll.VeterinaryPracticeRating.FirstOrDefaultAsync(id);

        if (rolesClaim.FirstOrDefault(claim => claim.Value == "Admin") == null && found!.UserId != user.Id)
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not have required claims and/or is not the owner of this comment", Status = HttpStatusCode.NotFound });
        }
        
        
        if (id == default) return BadRequest(new RestApiErrorResponse() { Error = "Id field not fulfilled", Status = HttpStatusCode.BadRequest});
        
        if (found == null) return NotFound(new RestApiErrorResponse() { Error = $"item with id: {id} was not found", Status = HttpStatusCode.NotFound});;
        await _bll.VeterinaryPracticeRating.RemoveAsync(found);
        await _bll.SaveChangesAsync();
        return Ok(true);
    }
    
    
    /// <summary>
    /// Add new rating the veterinary practice
    /// </summary>
    /// <returns>Veterinary Practice Rating</returns>
    [HttpPost]
    [Route("AddVeterinaryPracticeRating")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(VeterinaryPracticeRating), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<VeterinaryPracticeRating>> AddNewVeterinaryPracticeRating([FromBody] VeterinaryPracticeRating model)
    {
        var userEmailClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmailClaim == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound});
        }
    
        var userEmail = userEmailClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "User does not exist that was inside JWT", Status = HttpStatusCode.NotFound});
        }
        
        
        if (string.IsNullOrEmpty(model.Comment) ||
            model.VeterinaryPracticeId == default || model.Rating < 0)
        {
            return BadRequest(new RestApiErrorResponse() {Error = "One or more fields is empty", Status = HttpStatusCode.BadRequest});
        }
        try
        {
            var newExcerciseRating = new App.BLL.DTO.VeterinaryPracticeRating()
            {
                Comment = model.Comment,
                UserId = user.Id,
                VeterinaryPracticeId = model.VeterinaryPracticeId,
                Rating = model.Rating,
                PostedAt = DateTime.UtcNow
            };
            var added =_bll.VeterinaryPracticeRating.Add(newExcerciseRating);
            await _bll.SaveChangesAsync();
            return Ok(_mapper.Map(added));
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message, Status = HttpStatusCode.InternalServerError});
        }
    }
        
        
     /// <summary>
    /// Update VeterinaryPracticeRating  in the database
    /// </summary>
    /// <returns>Updated VeterinaryPracticeRating rating</returns>
    [HttpPut]
    [Route("UpdateVeterinaryPracticeRating")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(VeterinaryPracticeRating), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.Unauthorized)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<VeterinaryPracticeRating>> UpdateVeterinaryPRacticeRating([FromBody] VeterinaryPracticeRating model)
    {
        var found = await _bll.VeterinaryPracticeRating.FirstOrDefaultAsync(model.Id);
        var userEmailClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmailClaim == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound});
        }
    
        var userEmail = userEmailClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new RestApiErrorResponse {Error = "User does not exist that was inside JWT", Status = HttpStatusCode.NotFound});
        }
        
        if (found == null) return NotFound(new RestApiErrorResponse() { Error = $"item with id: {model.Id} was not found", Status = HttpStatusCode.NotFound});;

        if (found.UserId != user.Id)
            return Unauthorized(new RestApiErrorResponse()
                { Error = "You are not the owner of this comment", Status = HttpStatusCode.Unauthorized });        
        
        if (model.Id == default ||
            string.IsNullOrEmpty(model.Comment) || model.Rating < 0)
        {
            return BadRequest(new RestApiErrorResponse() {Error = "One or more fields is empty", Status = HttpStatusCode.BadRequest});
        }

        try
        {
            found.Comment = model.Comment;
            found.Rating = model.Rating;
            found.PostedAt = DateTime.UtcNow;
            
            var added =_bll.VeterinaryPracticeRating.Update(found);
            await _bll.SaveChangesAsync();
            return Ok(_mapper.Map(added));
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message, Status = HttpStatusCode.BadRequest});
        }
    }
    
    
    
}