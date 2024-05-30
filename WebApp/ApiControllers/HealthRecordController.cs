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
using HealthRecord = App.DTO.v1_0.HealthRecord;

namespace WebApp.ApiControllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class HealthRecordController :ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.HealthRecord, App.BLL.DTO.HealthRecord> _mapper;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="bll"></param>
    /// <param name="userManager"></param>
    /// <param name="autoMapper"></param>
    public HealthRecordController(AppDbContext context, IAppBLL bll, UserManager<AppUser> userManager,
        IMapper autoMapper)
    {
        _context = context;
        _bll = bll;
        _userManager = userManager;
        _mapper = new PublicDTOBllMapper<App.DTO.v1_0.HealthRecord, App.BLL.DTO.HealthRecord>(autoMapper);
    }

    /// <summary>
    /// Get All health records
    /// </summary>
    /// <returns>List of health Records</returns>
    [HttpGet]
    [Route("GetAllHealthRecords")]
    [ProducesResponseType<IEnumerable<App.DTO.v1_0.HealthRecord>>((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<App.DTO.v1_0.HealthRecord>>> GetHealthRecord()
    {
        var res = (await _bll.HealthRecord.GetAllHealthRecordwithoutcollectionIncludingAsync())
            .Select(e => _mapper.Map(e))
            .ToList();
        return Ok(res);
    }

    /// <summary>
    /// Get Specific health record
    /// </summary>
    /// <param name="id"></param>
    /// <returns>health record or not found</returns>
    [HttpGet]
    [Route("GetHealthRecord/{id}")]
    [ProducesResponseType<App.DTO.v1_0.HealthRecord>((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<App.DTO.v1_0.HealthRecord>> GetHealthrecord(Guid id)
    {
        var healthrecord = await _bll.HealthRecord.FirstOrDefaultAsyncInclude(id);

        if (healthrecord == null)
        {
            return NotFound();
        }

        return Ok(healthrecord);
    }

    /// <summary>
    ///Update healthrecord
    /// </summary>
    /// <param name="healthRecord"></param>
    /// <returns>updated healthrecord</returns>
    [HttpPatch]
    [Route("UpdateHealthRecord")]
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> PutHealthRecord( App.DTO.v1_0.HealthRecord healthRecord)
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
            var excisting = await _bll.HealthRecord.FirstOrDefaultAsync(healthRecord.Id);
            var newhealthrecord = new App.BLL.DTO.HealthRecord()
            {
                Id = excisting!.Id,
                AppUser = excisting.AppUser,
                AppUserId = excisting.AppUserId,
                Notes= healthRecord.Notes,
                Name = healthRecord.Name,
                Weight = healthRecord.Weight,
                PetId = healthRecord.PetId,
                VeterinaryPracticeId = healthRecord.VeterinaryPracticeId
            };

            var found = await _bll.HealthRecord.FirstOrDefaultAsync(healthRecord.Id);

            if (found == null) return NotFound(new RestApiErrorResponse() { Error = $"item with id: {healthRecord.Id} was not found", Status = HttpStatusCode.NotFound});;
            
            _bll.HealthRecord.Update(newhealthrecord);
            await _bll.SaveChangesAsync();
            return Ok(healthRecord);
        }
        catch (Exception)
        {
            return StatusCode(500); // Handle exceptions appropriately
        }
    }


    /// <summary>
    ///Add new healthrecord
    /// </summary>
    /// <param name="healthRecord"></param>
    /// <returns>added healthrecord</returns>
    [HttpPost]
    [Route("AddHealthRecord")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType<App.DTO.v1_0.HealthRecord>((int) HttpStatusCode.Created)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<App.DTO.v1_0.HealthRecord>> PostHealthRecord(App.DTO.v1_0.HealthRecord healthRecord)
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
        if (string.IsNullOrEmpty(healthRecord.Name) ||
            string.IsNullOrEmpty(healthRecord.Notes) )
        {
            return BadRequest(new RestApiErrorResponse() {Error = "One or more fields is empty", Status = HttpStatusCode.BadRequest});
        }
        try
        {
            var newHealthRecod = new App.BLL.DTO.HealthRecord()
            {
                Date = healthRecord.Date,
                Name = healthRecord.Name,
                Notes = healthRecord.Notes,
                Weight = healthRecord.Weight,
                PetId = healthRecord.PetId,
                VeterinaryPracticeId = healthRecord.VeterinaryPracticeId,
                AppUserId = user.Id
            };
            var added = _bll.HealthRecord.Add(newHealthRecod);
            await _bll.SaveChangesAsync();
            return Ok(_mapper.Map(added));

        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });
        }
    }


    /// <summary>
    ///Delete healthrecord
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("DeleteHealthRecord/{id}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> DeleteHealthRecord(Guid id)
    {
        var healthRecord = await _bll.HealthRecord.FirstOrDefaultAsyncInclude(id);
        if (healthRecord == null)
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

        try
        {
            if (user.Id == healthRecord.AppUserId)
            {
                await _bll.HealthRecord.RemoveAsync(healthRecord);
                await _bll.SaveChangesAsync();
                
                return NoContent();
            }
            else
            {
                return BadRequest(new RestApiErrorResponse()
                    { Error = "Cannot delete someone elses Health Record", Status = HttpStatusCode.BadRequest});
            }
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });
        }
    }
    /// <summary>
    /// Returns jwt owners pet health records
    /// </summary>
    /// <returns>Given pets healt records</returns>
    [HttpGet]
    [Route("GetPetHealthREcord")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(IEnumerable<HealthRecord>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<IEnumerable<App.DTO.v1_0.HealthRecord>>> GetPetHealthRecords()
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

        try
        {
            var pets = (await _bll.Pet.GetAllAsync()).Where(x => x.AppUserId == user.Id);
            if (pets == null)
            {
                return NotFound(new RestApiErrorResponse {Error = "NO PET found!", Status = HttpStatusCode.NotFound});

            }

            var pet = pets.FirstOrDefault();
            if (pet == null)
            {
                return NotFound(new RestApiErrorResponse {Error = "NO PET!", Status = HttpStatusCode.NotFound});

            }

            var petId = pet.Id;
            var found = (await _bll.HealthRecord.GetAllHealthRecordWithPetId()).Where(healthRecord => healthRecord.PetId == petId);
            
            return Ok(found);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Get jwt owners healthrecords
    /// </summary>
    /// <returns>list of healthrecords</returns>
    [HttpGet]
    [Route("GetUserHealthRecors")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(HealthRecord), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<HealthRecord>> GetUserHealthRecord()
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
        
        var found = (await _bll.HealthRecord.GetAllHealthRecordwithoutcollectionIncludingAsync()).Where(hr => hr.AppUserId == user.Id);
        
        return Ok(found);
    }

}