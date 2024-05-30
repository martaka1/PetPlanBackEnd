using System.Net;
using App.Contracts.BLL;
using App.Domain.Identity;
using App.DTO.v1_0;
using Asp.Versioning;
using AutoMapper;
using DAL.App.EF;
using Domain.App;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Helpers;
using BlogPost = App.BLL.DTO.BlogPost;

namespace WebApp.ApiControllers;
/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class BlogTagController :ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.BlogTag, App.BLL.DTO.BlogTag> _mapper;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="bll"></param>
    /// <param name="userManager"></param>
    /// <param name="autoMapper"></param>
    public BlogTagController(AppDbContext context, IAppBLL bll, UserManager<AppUser> userManager,
        IMapper autoMapper)
    {
        _context = context;
        _bll = bll;
        _userManager = userManager;
        _mapper = new PublicDTOBllMapper<App.DTO.v1_0.BlogTag, App.BLL.DTO.BlogTag>(autoMapper);
    }

    /// <summary>
    /// Get all post tags
    /// </summary>
    /// <returns>List of post tags</returns>
    [HttpGet]
    [Route("AllBlogTags")]
    [ProducesResponseType<IEnumerable<App.DTO.v1_0.BlogTag>>((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<App.DTO.v1_0.BlogTag>>> GetBlogTags()
    {
        var res = (await _bll.BlogTag.GetAllBlogTagsIncludedAsync())
            .Select(e => _mapper.Map(e))
            .ToList();
        return Ok(res);
    }

    /// <summary>
    /// REturn specific blog tag
    /// </summary>
    /// <param name="id"></param>
    /// <returns>specific blog tag or not found</returns>
    [HttpGet]
    [Route("GetBlogTag/{id}")]
    [ProducesResponseType<App.DTO.v1_0.BlogTag>((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<App.DTO.v1_0.BlogTag>> GetBlogTag(Guid id)
    {
        var blogTag = await _bll.BlogTag.FirstOrDefaultAsync(id);

        if (blogTag == null)
        {
            return NotFound();
        }

        return Ok(blogTag);
    }
    
    /// <summary>
    ///Update blog tag
    /// </summary>
    /// <param name="id"></param>
    /// <param name="blogTag"></param>
    /// <returns>updated blog tag</returns>
    [HttpPut]
    [Route("UpdateBlogTag/{id}")]
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> PutBlogTag(Guid id, App.DTO.v1_0.BlogTag blogTag)
    {
        var found = await _bll.BlogTag.FirstOrDefaultAsync(id);
        if (found == null)
        {
            return NotFound();
        }

        try
        {
            var updatedBlogTag = new App.BLL.DTO.BlogTag()
            {
                Id=found.Id,
                Name = blogTag.Name
            };
            _bll.BlogTag.Update(updatedBlogTag);  
            await _bll.SaveChangesAsync();
            return NoContent(); 

        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }


    /// <summary>
    ///Add new blog tag
    /// </summary>
    /// <param name="blogTag"></param>
    /// <returns>New blog tag</returns>
    [HttpPost]
    [Route("AddBlogTag")]
    [ProducesResponseType<App.DTO.v1_0.BlogTag>((int) HttpStatusCode.Created)]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<App.DTO.v1_0.BlogTag>> PostBlogTag(App.DTO.v1_0.BlogTag blogTag)
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

        if (string.IsNullOrEmpty(blogTag.Name))
        {
            return BadRequest(new RestApiErrorResponse()
                { Error = "One or more fields is empty", Status = HttpStatusCode.BadRequest});
        }

        try
        {
            var newBlogTag = new App.BLL.DTO.BlogTag()
            {
                Name = blogTag.Name
            };
            var added = _bll.BlogTag.Add(newBlogTag);
            await _bll.SaveChangesAsync();
            return Ok(_mapper.Map(added));

        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });
        }
    }

    /// <summary>
    ///Delete blog tag
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true or false</returns>
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [HttpDelete]
    [Route("DeleteBlogTag/{id}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> DeleteBlogTag(Guid id)
    {
        var blogTag = await _bll.BlogTag.FirstOrDefaultAsync(id);
        if (blogTag == null)
        {
            return NotFound();
        }

        try
        {
            await _bll.BlogTag.RemoveAsync(blogTag);
            await _bll.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });
        }
    }
}