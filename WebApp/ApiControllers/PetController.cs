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

public class PetController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAppBLL _bll;
    private readonly UserManager<AppUser> _userManager;
    private readonly PublicDTOBllMapper<App.DTO.v1_0.Pet, App.BLL.DTO.Pet> _mapper;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="bll"></param>
    /// <param name="userManager"></param>
    /// <param name="autoMapper"></param>
    public PetController(AppDbContext context, IAppBLL bll, UserManager<AppUser> userManager,
        IMapper autoMapper)
    {
        _context = context;
        _bll = bll;
        _userManager = userManager;
        _mapper = new PublicDTOBllMapper<App.DTO.v1_0.Pet, App.BLL.DTO.Pet>(autoMapper);
    }


    /// <summary>
    /// Returns all pets in the database
    /// </summary>
    /// <returns>all pets in the database</returns>
    [HttpGet]
    [ProducesResponseType<IEnumerable<App.DTO.v1_0.PetWithoutCollection>>((int)HttpStatusCode.OK)]
    [Route("AllPets")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<List<App.DTO.v1_0.PetWithoutCollection>>> GetPets()
    {
        var res = (await _bll.Pet.GetWithoutCollectionPetIncludingAsync());
        return Ok(res);
    }


/// <summary>
    /// Returns one specific pet
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Specific pet or not found</returns>
    [HttpGet]
    [Route("GetPet/{id}")]
    [ProducesResponseType<App.DTO.v1_0.Pet>((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<ActionResult<App.DTO.v1_0.Pet>> GetPet(Guid id)
    {
        if (id == default) return BadRequest(id);
        var found = (await _bll.Pet.GetAllPetIncludingAsync()).FirstOrDefault(p => p.Id == id);

        if (found == null) return NotFound(new RestApiErrorResponse {Error = $"Pet with id: {id} not found", Status = HttpStatusCode.NotFound});
        return Ok(found);
    }

    /// <summary>
    ///Update excisting pet
    /// </summary>
    /// <param name="pet"></param>
    /// <returns>Update pet</returns>
    [HttpPatch]
    [Route("UpdatePet")]
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Consumes("application/json")]
    public async Task<IActionResult> UpdatePet( App.DTO.v1_0.Pet pet)
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
            var excisting = await _bll.Pet.FirstOrDefaultAsync(pet.Id);
            var newPet = new App.BLL.DTO.Pet()
            {
                Id = excisting!.Id,
                AppUser = excisting.AppUser,
                AppUserId = excisting.AppUserId,
                Breed = pet.Breed,
                ChipNr = pet.ChipNr,
                PetName = pet.PetName,
                Spices = pet.Spices
            };

            var found = await _bll.Pet.FirstOrDefaultAsync(pet.Id);

            if (found == null) return NotFound(new RestApiErrorResponse() { Error = $"item with id: {pet.Id} was not found", Status = HttpStatusCode.NotFound});;
            
            _bll.Pet.Update(newPet);
            await _bll.SaveChangesAsync();
            return Ok(pet);
        }
        catch (Exception)
        {
            return StatusCode(500); // Handle exceptions appropriately
        }
    }


    /// <summary>
    ///Add new pet
    /// </summary>
    /// <returns>List of Pets</returns>
    [HttpPost]
    [Route("AddPet")]
    [ProducesResponseType<App.DTO.v1_0.Pet>((int) HttpStatusCode.Created)]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.BadRequest)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<App.DTO.v1_0.Pet>> PostPet([FromBody]Pet pet)
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
        if (string.IsNullOrEmpty(pet.Breed) ||
            string.IsNullOrEmpty(pet.PetName) ||
            string.IsNullOrEmpty(pet.Spices) ||
            pet.ChipNr == 0 ||
            pet.ChipNr<0||
            pet.Dob == DateTime.MinValue)
        {
            return BadRequest(new RestApiErrorResponse()
                { Error = "One or more fields is empty", Status = HttpStatusCode.BadRequest});
        }

        try
        {
            var newPet = new App.BLL.DTO.Pet()
            {
                AppUserId = user.Id,
                Breed = pet.Breed,
                ChipNr = pet.ChipNr,
                Dob = pet.Dob,
                PetName = pet.PetName,
                Spices = pet.Spices
            };
            var added = _bll.Pet.Add(newPet);
            await _bll.SaveChangesAsync();
            return Ok(_mapper.Map(added));
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });
        }
    }

    /// <summary>
    ///Delete pet
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true or false depending on if it did its job</returns>
    [ProducesResponseType((int) HttpStatusCode.NoContent)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [HttpDelete]
    [Route("DeletePet/{id}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeletePet(Guid id)
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
        var pet = await _bll.Pet.FirstOrDefaultAsync(id);
        if (pet == null)
        {
            return NotFound();
        }

        try
        {
            if (user.Id == pet.AppUserId)
            {
                await _bll.Pet.RemoveAsync(pet);
                await _bll.SaveChangesAsync();

                return NoContent();
            }
            else
            {
                return BadRequest(new RestApiErrorResponse()
                    { Error = "Cannot delete someone elses pet", Status = HttpStatusCode.BadRequest});
            }
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse() { Error = e.Message });
        }
    }
    /// <summary>
    /// Get jwt owners pets
    /// </summary>
    /// <param></param>
    /// <returns>list of pets</returns>
    [HttpGet]
    [Route("GetUserPets")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(PetWithoutCollection), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), (int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    [Consumes("application/json")]

    public async Task<ActionResult<PetWithoutCollection>> GetUserPets()
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
        
        var found = (await _bll.Pet.GetWithoutCollectionPetIncludingAsync()).Where(pet => pet.AppUserId == user.Id);
        
        return Ok(found);
    }
}