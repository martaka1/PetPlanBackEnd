using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;
using Domain.App;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.Identity;

public class AppUser : IdentityUser<Guid>, IDomainEntityId
{
    [MinLength(1)]
    [MaxLength(64)]
    public string FirstName { get; set; } = default!;

    [MinLength(1)]
    [MaxLength(64)]
    public string LastName { get; set; } = default!;
    
    public ICollection<Pet>? Pet { get; set; }
    public ICollection<HealthRecord>? HealthRecords;

    public ICollection<AppRefreshToken>? RefreshTokens { get; set; }
}