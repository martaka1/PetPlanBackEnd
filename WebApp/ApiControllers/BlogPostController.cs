using System.Net;
using System.Security.Claims;
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
using WebApp.Helpers;

namespace WebApp.ApiControllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]

public class BlogPostController :ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.BlogPost, App.BLL.DTO.BlogPost> _mapper;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="bll"></param>
    /// <param name="userManager"></param>
    /// <param name="autoMapper"></param>
    public BlogPostController(AppDbContext context, IAppBLL bll, UserManager<AppUser> userManager,
        IMapper autoMapper)
    {
        _context = context;
        _bll = bll;
        _userManager = userManager;
        _mapper = new PublicDTOBllMapper<App.DTO.v1_0.BlogPost, App.BLL.DTO.BlogPost>(autoMapper);
    }

    /// <summary>
    /// Retruns all blogposts
    /// </summary>
    /// <returns>List of blogposts</returns>
    [HttpGet]
    [Route("AllBlogPosts")]
    [ProducesResponseType<IEnumerable<App.DTO.v1_0.BlogPostWithoutCollection>>((int) HttpStatusCode.OK)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<App.DTO.v1_0.BlogPostWithoutCollection>>> GetBlogPosts()
    {
        var res = (await _bll.BlogPost.GetWithoutCollectionBlogPostIncludingAsync())
            .Select(e => _mapper.Map(e))
            .ToList();
        return Ok(res);
    }

    /// <summary>
    /// Retrun specific blogpost from database
    /// </summary>
    /// <param name="id"></param>
    /// <returns> BlogPost or not found</returns>
    [HttpGet]
    [Route("GetBlogPost/{id}")]
    [ProducesResponseType<App.DTO.v1_0.BlogPost>((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<App.DTO.v1_0.BlogPost>> GetBlogPost(Guid id)
    {
        var blogpost = (await _bll.BlogPost.GetWithoutCollectionBlogPostIncludingAsync()).FirstOrDefault(bp => bp.Id == id);

        if (blogpost == null)
        {
            return NotFound();
        }

        return Ok(blogpost);
    }

    /// <summary>
    /// Get jwt owners blogposts
    /// </summary>
    /// <returns>list of blogpost</returns>
    [HttpGet]
    [Route("GetUserBlogposts")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(BlogPost), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<BlogPost>> GetUserBlogPosts()
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
        
        var found = (await _bll.BlogPost.GetWithoutCollectionBlogPostIncludingAsync()).Where(bp => bp.AppUserId == user.Id);
        
        return Ok(found);
    }

    ///  <summary>
    /// Update Blog post
    ///  </summary>
    ///  <param name="blogPost"></param>
    ///  <returns>Updated blogpost</returns>
    [HttpPatch]
    [Route("UpdateBlogPost")]
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> PutBlogPost( App.DTO.v1_0.BlogPost blogPost)
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
            var excisting = await _bll.BlogPost.FirstOrDefaultAsync(blogPost.Id);
            var newCategory = new App.BLL.DTO.BlogPost()
            {
                Id = excisting!.Id,
                AppUser = excisting.AppUser,
                AppUserId = excisting.AppUserId,
                Content=blogPost.Content,
                Title = blogPost.Title,
                Summary = blogPost.Summary,
                Tags = excisting.Tags,
                Comments = excisting.Comments
            };

            var found = await _bll.BlogPost.FirstOrDefaultAsync(blogPost.Id);

            if (found == null) return NotFound(new RestApiErrorResponse() { Error = $"item with id: {blogPost.Id} was not found", Status = HttpStatusCode.NotFound});;
            
            _bll.BlogPost.Update(newCategory);
            await _bll.SaveChangesAsync();
            return Ok(blogPost);
        }
        catch (Exception)
        {
            return StatusCode(500); // Handle exceptions appropriately
        }
    }


    /// <summary>
    ///Add new blogpost
    /// </summary>
    /// <param name="blogPost"></param>
    /// <returns>BlogPostThat was made</returns>
    [HttpPost]
    [Route("AddBlogPost")]
    [ProducesResponseType<App.BLL.DTO.BlogPost>((int) HttpStatusCode.Created)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<App.BLL.DTO.BlogPost>> PostBlogPost(App.BLL.DTO.BlogPost blogPost)
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
        if (string.IsNullOrEmpty(blogPost.Title) ||
            string.IsNullOrEmpty(blogPost.Summary) ||
            string.IsNullOrEmpty(blogPost.Content) )
        {
            return BadRequest(new RestApiErrorResponse() {Error = "One or more fields is empty", Status = HttpStatusCode.BadRequest});
        }
        try
        {
            var newBlogPost = new App.BLL.DTO.BlogPost()
            {
                AppUserId = user.Id,
                Content = blogPost.Content,
                Date = DateTime.UtcNow,
                Summary = blogPost.Summary,
                Title = blogPost.Title
                
            };
            var added = _bll.BlogPost.Add(newBlogPost);
            await _bll.SaveChangesAsync();
            return Ok(_mapper.Map(added));
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });
        }
    }

    /// <summary>
    ///Delete blogPost
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true or false depending on if it did its job</returns>
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [HttpDelete]
    [Route("DeleteBlogPost/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> DeleteBlogPost(Guid id)
    {
        var blogPost = await _bll.BlogPost.FirstOrDefaultAsync(id);
        if (blogPost == null)
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
            if (user.Id == blogPost.AppUserId || isAdmin)
            {
                await _bll.BlogPost.RemoveAsync(blogPost);
                await _bll.SaveChangesAsync();
                return NoContent();
            }

            else
            {
                return BadRequest(new RestApiErrorResponse()
                    
                    { Error = "Cannot delete someone elses blogPost", Status = HttpStatusCode.BadRequest});
            }
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });
        }
    }

}