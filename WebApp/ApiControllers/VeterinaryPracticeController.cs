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

namespace WebApp.ApiControllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class VeterinaryPracticeController : ControllerBase
{
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly PublicDTOBllMapper<VeterinaryPractice, App.BLL.DTO.VeterinaryPractice> _mapper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="bll"></param>
    /// <param name="userManager"></param>
    /// <param name="autoMapper"></param>
    public VeterinaryPracticeController(AppDbContext context, IAppBLL bll, UserManager<AppUser> userManager,
        IMapper autoMapper)
    {
        _bll = bll;
        _userManager = userManager;
        _mapper = new PublicDTOBllMapper<App.DTO.v1_0.VeterinaryPractice, App.BLL.DTO.VeterinaryPractice>(autoMapper);
    }

    /// <summary>
    /// Return a list of Veterinary practices
    /// </summary>
    /// <returns>List of veterinary practices</returns>
    [HttpGet]
    [Route("AllVeterinaryPractices")]
    [ProducesResponseType<IEnumerable<App.DTO.v1_0.VeterinaryPractice>>((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<App.DTO.v1_0.VeterinaryPractice>>> GetVeterinaryPractices()
    {
        var res = (await _bll.VeterinaryPractice.GetAllVeterinaryPracticeWithoutCollectionsIncludingAsync())
            .Select(e => _mapper.Map(e))
            .ToList();
        return Ok(res);
    }


    /// <summary>
    ///REtruns a specific Veterinary Practice
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Veterinary practice or not found</returns>
    [HttpGet]
    [Route("GetVeterinaryPractice/{id}")]
    [ProducesResponseType<App.DTO.v1_0.VeterinaryPracticeWithoutCollection>((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<App.DTO.v1_0.VeterinaryPracticeWithoutCollection>> GetVeterinaryPractice(Guid id)
    {
        var veterinaryPractice = (await _bll.VeterinaryPractice.GetAllVeterinaryPracticeWithoutCollectionsIncludingAsync()).FirstOrDefault(vp => vp.Id == id);

        if (veterinaryPractice == null)
        {
            return NotFound();
        }

        return Ok(veterinaryPractice);
    }

    /// <summary>
    ///Update Veterinarypractice
    /// </summary>
    /// <param name="id"></param>
    /// <param name="veterinaryPractice"></param>
    /// <returns>Updated veterinary practice</returns>
    [HttpPatch]
    [Route("UpdateVeterinaryPractice")]
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> UpdateVeterinaryPractice(Guid id, App.DTO.v1_0.VeterinaryPractice veterinaryPractice)
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
            var excisting = await _bll.VeterinaryPractice.FirstOrDefaultAsync(veterinaryPractice.Id);
            var updatedVeterinaryPractice = new App.BLL.DTO.VeterinaryPractice()
            {
                Id=excisting!.Id,
                Location = veterinaryPractice.Location,
                PhoneNr = veterinaryPractice.PhoneNr,
                RegistrationNr = veterinaryPractice.RegistrationNr,
                VeterinaryPracticeName = veterinaryPractice.VeterinaryPracticeName
            };
            _bll.VeterinaryPractice.Update(updatedVeterinaryPractice);
            await _bll.SaveChangesAsync();
            
            return Ok(veterinaryPractice);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
        
    }

    /// <summary>
    ///Add new Veterinary Practice
    /// </summary>
    /// <param name="veterinaryPractice"></param>
    /// <returns>Added practice</returns>
    [HttpPost]
    [Route("AddVeterinaryPractice")]
    [ProducesResponseType<App.DTO.v1_0.VeterinaryPractice>((int) HttpStatusCode.Created)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<App.DTO.v1_0.VeterinaryPractice>> PostVeterinaryPractice(App.DTO.v1_0.VeterinaryPractice veterinaryPractice)
    {
        if (string.IsNullOrEmpty(veterinaryPractice.VeterinaryPracticeName) ||
            string.IsNullOrEmpty(veterinaryPractice.Location) ||
            string.IsNullOrEmpty(veterinaryPractice.PhoneNr) ||
            veterinaryPractice.RegistrationNr == 0)
        {
            return BadRequest(new RestApiErrorResponse()
                { Error = "One or more fields is empty", Status = HttpStatusCode.BadRequest});
        }

        try
        {
            var newVetertinaryPractice = new App.BLL.DTO.VeterinaryPractice()
            {
                Location = veterinaryPractice.Location,
                PhoneNr = veterinaryPractice.PhoneNr,
                RegistrationNr = veterinaryPractice.RegistrationNr,
                VeterinaryPracticeName = veterinaryPractice.VeterinaryPracticeName
            };
            var added = _bll.VeterinaryPractice.Add(newVetertinaryPractice);
            await _bll.SaveChangesAsync();
            return Ok(_mapper.Map(added));

        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });
        }
    }

    /// <summary>
    ///Delete VeterinaryPractice
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true or false</returns>
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [HttpDelete]
    [Route("DeleteVeterinaryPractice/{id}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> DeleteVeterinaryPractice(Guid id)
    {
        var veterinaryPractice = await _bll.VeterinaryPractice.FirstOrDefaultAsync(id);
        if (veterinaryPractice == null)
        {
            return NotFound();
        }

        try
        {
            await _bll.VeterinaryPractice.RemoveAsync(veterinaryPractice);
            await _bll.SaveChangesAsync();
            
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });
        }
    }
}