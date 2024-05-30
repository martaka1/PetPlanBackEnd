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
[Route("api/v{version:apiVersion}/ExcerciseRating")]
public class ExcerciseRatingController : ControllerBase
{
    
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.ExcerciseRating, App.BLL.DTO.ExcerciseRating> _mapper;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bll"></param>
    /// <param name="userManager"></param>
    /// <param name="mapper"></param>
    public ExcerciseRatingController(IAppBLL bll, UserManager<AppUser> userManager, IMapper mapper)
    {
        _bll = bll;
        _userManager = userManager;
        _mapper = new PublicDTOBllMapper<ExcerciseRating, App.BLL.DTO.ExcerciseRating>(mapper);
    }
    
    
    /// <summary>
    /// Returns all excercise ratings from the database
    /// </summary>
    /// <returns>list of excercise ratings</returns>
    [HttpGet]
    [Route("AllExcerciseRating")]
    [ProducesResponseType(typeof(IEnumerable<ExcerciseRating>), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<IEnumerable<ExcerciseRating>>> GetAllExcerciseRatings()
    {
        var found = (await _bll.ExcerciseRating.GetAllExcercisesRatingsCommentsIncludingAsync())
            .Select(rating => _mapper.Map(rating));
      
        return Ok(found);
    }
    
    
    
    /// <summary>
    /// Returns specific excercise rating from the database
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Specific excercise rating or not found</returns>
    [HttpGet]
    [Route("GetExcerciseRating/{id}")]
    [ProducesResponseType(typeof(ExcerciseRating), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<ExcerciseRating>> GetExcerciseRating(Guid id)
    {
        if (id == default) return BadRequest(id);
        var found = (await _bll.ExcerciseRating.GetAllExcercisesRatingsCommentsIncludingAsync()).FirstOrDefault(kb => kb.Id == id);

        if (found == null) return NotFound(new RestApiErrorResponse {Error = $"Rating with id: {id} was not found", Status = HttpStatusCode.NotFound});
        return Ok(_mapper.Map(found));
    }
    
    
        /// <summary>
    /// Returns speicific user ExcerciseRating 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>List of ExcerciseRating </returns>
    [HttpGet]
    [Route("GetUserExcerciseRating/")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ExcerciseRating), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<IEnumerable<ExcerciseRating>>> GetUserExcerciseRatings()
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
        
        var found = (await _bll.ExcerciseRating.GetAllExcercisesRatingsCommentsIncludingAsync()).Where(bp => bp.UserId == user.Id);
        
        return Ok(found);
    }
        
    
    /// <summary>
    /// Delete ExcerciseRating from the database
    /// </summary>
    /// <returns>true or false depending on if it did its job</returns>
    [HttpDelete]
    [Route("DeleteExcerciseRating/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<ExcerciseRating>> DeleteExcerciseRating(Guid id)
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
        
        var found = await _bll.ExcerciseRating.FirstOrDefaultAsync(id);

        if (rolesClaim.FirstOrDefault(claim => claim.Value == "Admin") == null && found!.UserId != user.Id)
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not have required claims and/or is not the owner of this comment", Status = HttpStatusCode.NotFound });
        }
        
        
        if (id == default) return BadRequest(new RestApiErrorResponse() { Error = "Id field not fulfilled", Status = HttpStatusCode.BadRequest});;
        
        if (found == null) return NotFound(new RestApiErrorResponse() { Error = $"item with id: {id} was not found", Status = HttpStatusCode.NotFound});;
        await _bll.ExcerciseRating.RemoveAsync(found);
        await _bll.SaveChangesAsync();
        return Ok(true);
    }
    
    
    /// <summary>
    /// Add new rating the ExcerciseRating
    /// </summary>
    /// <returns>ExcerciseRating</returns>
    [HttpPost]
    [Route("AddExcerciseRating")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ExcerciseRating), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<ExcerciseRating>> AddNewExcerciseRating([FromBody] ExcerciseRating excerciseRating)
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
        
        
        if (string.IsNullOrEmpty(excerciseRating.Comment) ||
            excerciseRating.ExcerciseId == default || excerciseRating.Rating < 0 || excerciseRating.Rating>5)
        {
            return BadRequest(new RestApiErrorResponse() {Error = "One or more fields is empty", Status = HttpStatusCode.BadRequest});
        }
        try
        {
            var newExcerciseRating = new App.BLL.DTO.ExcerciseRating()
            {
                Comment = excerciseRating.Comment,
                UserId = user.Id,
                ExcerciseId = excerciseRating.ExcerciseId,
                Rating = excerciseRating.Rating,
                PostedAt = DateTime.UtcNow
            };
            var added =_bll.ExcerciseRating.Add(newExcerciseRating);
            await _bll.SaveChangesAsync();
            return Ok(_mapper.Map(added));
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message, Status = HttpStatusCode.InternalServerError});
        }
    }
        
        
     /// <summary>
    /// Update ExcerciseRating  in the database
    /// </summary>
    /// <returns>Updated ExcerciseRating rating</returns>
    [HttpPut]
    [Route("UpdateExcerciseRating")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ExcerciseRating), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.Unauthorized)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<ExcerciseRating>> UpdateExcerciseRating([FromBody] ExcerciseRating model)
    {
        var found = await _bll.ExcerciseRating.FirstOrDefaultAsync(model.Id);
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
            
            var added =_bll.ExcerciseRating.Update(found);
            await _bll.SaveChangesAsync();
            return Ok(_mapper.Map(added));
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message, Status = HttpStatusCode.BadRequest});
        }
    }
    
    
    
}