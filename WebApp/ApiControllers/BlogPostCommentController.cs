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
[Route("api/v{version:apiVersion}/BlogPostComment")]
public class BlogPostCommentController : ControllerBase
{
    
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly PublicDTOBllMapper<BlogPostComment, App.BLL.DTO.BlogPostComment> _mapper;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bll"></param>
    /// <param name="userManager"></param>
    /// <param name="mapper"></param>
    public BlogPostCommentController(IAppBLL bll, UserManager<AppUser> userManager, IMapper mapper)
    {
        _bll = bll;
        _userManager = userManager;
        _mapper = new PublicDTOBllMapper<BlogPostComment, App.BLL.DTO.BlogPostComment>(mapper);
    }
    
    
    /// <summary>
    /// Returns all BlogPostComment from the database
    /// </summary>
    /// <returns/>list of home BlogPostComment/returns>
    [HttpGet]
    [Route("AllBlogPostComment")]
    [ProducesResponseType(typeof(IEnumerable<BlogPostComment>), (int)HttpStatusCode.OK)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<IEnumerable<BlogPostComment>>> GetAllBlogPostComments()
    {
        var found = (await _bll.BlogPostComment.GetAllBlogPostCommentsIncludingAsync())
            .Select(rating => _mapper.Map(rating));
      
        return Ok(found);
    }
    
    
    
    /// <summary>
    /// Returns specific BlogPostComment from the database
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Specific BlogPostComment or not found</returns>
    [HttpGet]
    [Route("GetBlogPostComment/{id}")]
    [ProducesResponseType(typeof(BlogPostComment), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<BlogPostComment>> GetBlogPostComment(Guid id)
    {
        if (id == default) return BadRequest(id);
        var found = (await _bll.BlogPostComment.GetAllBlogPostCommentsIncludingAsync()).FirstOrDefault(kb => kb.Id == id);

        if (found == null) return NotFound(new RestApiErrorResponse {Error = $"Rating with id: {id} was not found", Status = HttpStatusCode.NotFound});
        return Ok(_mapper.Map(found));
    }
    
    
        /// <summary>
    /// Returns speicific user BlogPostComment 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>List of BlogPostComment </returns>
    [HttpGet]
    [Route("GetUserBlogPostComment/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(BlogPostComment), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<IEnumerable<BlogPostComment>>> GetUserBlogPostComment(Guid id)
    {
        if (id == default) return BadRequest(id);
        var found = (await _bll.BlogPostComment.GetAllBlogPostCommentsIncludingAsync())
            .Where(er => er.User!.Id == id)
            .Select(rating => _mapper.Map(rating));
        
        return Ok(found);
    }
        
    
    /// <summary>
    /// Delete BlogPostComment from the database
    /// </summary>
    /// <returns>true or false depending on if it did its job</returns>
    [HttpDelete]
    [Route("DeleteBlogPostComment/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<BlogPostComment>> DeleteBlogPostComment(Guid id)
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
        
        var found = await _bll.BlogPostComment.FirstOrDefaultAsync(id);

        if (rolesClaim.FirstOrDefault(claim => claim.Value == "Admin") == null && found!.UserId != user.Id)
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not have required claims and/or is not the owner of this comment", Status = HttpStatusCode.NotFound });
        }
        
        
        if (id == default) return BadRequest(new RestApiErrorResponse() { Error = "Id field not fulfilled", Status = HttpStatusCode.BadRequest});;
        
        if (found == null) return NotFound(new RestApiErrorResponse() { Error = $"item with id: {id} was not found", Status = HttpStatusCode.NotFound});;
        await _bll.BlogPostComment.RemoveAsync(found);
        await _bll.SaveChangesAsync();
        return Ok(true);
    }
    
    
    /// <summary>
    /// Add new rating the BlogPostComment
    /// </summary>
    /// <returns>BlogPostComment</returns>
    [HttpPost]
    [Route("AddBlogPostComment")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(BlogPostComment), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<BlogPostComment>> AddNewBlogPostComment([FromBody] BlogPostComment blogPostComment)
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
        
        
        if (string.IsNullOrEmpty(blogPostComment.Comment) ||
            blogPostComment.BlogPostId == default )
        {
            return BadRequest(new RestApiErrorResponse() {Error = "One or more fields is empty", Status = HttpStatusCode.BadRequest});
        }
        try
        {
            var newBlogPostComment = new App.BLL.DTO.BlogPostComment()
            {
                Comment = blogPostComment.Comment,
                UserId = user.Id,
                BlogPostId = blogPostComment.BlogPostId,
                PostedAt = DateTime.UtcNow
            };
            var added =_bll.BlogPostComment.Add(newBlogPostComment);
            await _bll.SaveChangesAsync();
            return Ok(_mapper.Map(added));
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message, Status = HttpStatusCode.InternalServerError});
        }
    }
        
        
     /// <summary>
    /// Update BlogPostComment  in the database
    /// </summary>
    /// <returns>Updated BlogPostComment rating</returns>
    [HttpPut]
    [Route("UpdateBlogPostComment")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(BlogPostComment), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.Unauthorized)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<BlogPostComment>> UpdateBlogPostComment([FromBody] BlogPostComment blogPostComment)
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
            var excisting = await _bll.BlogPostComment.FirstOrDefaultAsync(blogPostComment.Id);
            var newBlogPostComment = new App.BLL.DTO.BlogPostComment()
            {
                Id = excisting!.Id,
                User = excisting.User,
                UserId = excisting.UserId,
                Comment= blogPostComment.Comment,
                BlogPostId = excisting.BlogPostId
            };

            var found = await _bll.BlogPostComment.FirstOrDefaultAsync(blogPostComment.Id);

            if (found == null) return NotFound(new RestApiErrorResponse() { Error = $"item with id: {blogPostComment.Id} was not found", Status = HttpStatusCode.NotFound});;
            
            _bll.BlogPostComment.Update(newBlogPostComment);
            await _bll.SaveChangesAsync();
            return Ok(blogPostComment);
        }
        catch (Exception)
        {
            return StatusCode(500); // Handle exceptions appropriately
        }
    }
    
    
    
}