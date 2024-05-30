using System.Net;
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
using PostTags = App.DTO.v1_0.PostTags;

namespace WebApp.ApiControllers;

/// <summary>
/// Manages post tags.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class PostTagsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.PostTags, App.BLL.DTO.PostTags> _mapper;

    /// <summary>
    /// Constructor for PostTagsController.
    /// </summary>
    public PostTagsController(AppDbContext context, IAppBLL bll, UserManager<AppUser> userManager, IMapper autoMapper)
    {
        _context = context;
        _bll = bll;
        _userManager = userManager;
        _mapper = new PublicDTOBllMapper<App.DTO.v1_0.PostTags, App.BLL.DTO.PostTags>(autoMapper);
    }

    /// <summary>
    /// Get all postt ags from db
    /// </summary>
    /// <returns>List of post tags</returns>
    [HttpGet]
    [Route("GetAllBlogPostTags")]
    [ProducesResponseType(typeof(IEnumerable<App.DTO.v1_0.PostTags>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<List<App.DTO.v1_0.PostTags>>> GetPostTags()
    {
        var res = (await _bll.PostTag.GetAllAsync())
            .Select(e => _mapper.Map(e)).ToList();
        return Ok(res);
    }

    /// <summary>
    /// Add new blog tag
    /// </summary>
    /// <param name="model"></param>
    /// <returns>New blog tag</returns>
    [HttpPost]
    [Route("AddBlogPostTag")]
    [ProducesResponseType(typeof(App.DTO.v1_0.PostTags), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<App.DTO.v1_0.PostTags>> PostPostTags(App.DTO.v1_0.PostTags model)
    {
        try
        {
            if (model.BlogPostId == default || model.TagId == default)
            {
                return BadRequest(new RestApiErrorResponse()
                    { Error = "One or more fields is empty", Status = HttpStatusCode.BadRequest });
            }

            try
            {
                var postTag = new App.BLL.DTO.PostTags()
                {
                    BlogPostId = model.BlogPostId,
                    TagId = model.TagId
                };
                var added = _bll.PostTag.Add(postTag);
                await _bll.SaveChangesAsync();
                return Ok(_mapper.Map(added));
            }
            catch (Exception e)
            {
                return BadRequest(new RestApiErrorResponse() { Error = e.Message, Status = HttpStatusCode.BadRequest});
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, new RestApiErrorResponse()
            {
                Error = "Something went wrong contact software developer", Status = HttpStatusCode.InternalServerError
            });
        }
    }

    /// <summary>
    /// DElete blog taag
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("DeletePostTag/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<bool>> DeletePostTag(Guid id)
    {
        var userEmailClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        if (userEmailClaim == null)
        {
            return NotFound(new RestApiErrorResponse { Error = "JWT does not contain email claim", Status = HttpStatusCode.NotFound });
        }

        var userEmail = userEmailClaim.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (id == default) return BadRequest(new RestApiErrorResponse { Error = "Invalid ID", Status = HttpStatusCode.BadRequest });

        var found = await _bll.PostTag.FirstOrDefaultAsync(id);
        if (found == null) return NotFound(new RestApiErrorResponse { Error = "PostTag not found", Status = HttpStatusCode.NotFound });

        await _bll.PostTag.RemoveAsync(found);
        await _bll.SaveChangesAsync();

        return Ok(true);
    }

    /// <summary>
    /// Get post tags
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetPostTags/{id}")]
    [ProducesResponseType(typeof(PostTags), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<PostTags>> GetPostTag(Guid id)
    {
        if (id == default) return BadRequest(new RestApiErrorResponse { Error = "Invalid ID", Status = HttpStatusCode.BadRequest });

        var found = (await _bll.PostTag.GetAllPostTagIncludingAsync()).FirstOrDefault(postTags => postTags.BlogPostId == id);
        if (found == null) return NotFound(new RestApiErrorResponse { Error = $"PostTag with id: {id} not found", Status = HttpStatusCode.NotFound });

        return Ok(_mapper.Map(found));
    }
}
