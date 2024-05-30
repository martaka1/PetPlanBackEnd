using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace App.DAL.DTO.Identity;

public class AppUser:IdentityUser<Guid>
{
    [MinLength(1)]
    [MaxLength(64)]
    public string FirstName { get; set; } = default!;

    [MinLength(1)]
    [MaxLength(64)]
    public string LastName { get; set; } = default!;
    
    public ICollection<Pet>? Pet { get; set; }

    public ICollection<AppRefreshToken>? RefreshTokens { get; set; }
}