using System.ComponentModel.DataAnnotations;

namespace App.BLL.DTO.Identity;

public class AppUser
{
    [MinLength(1)]
    [MaxLength(128)]
    public string FirstName { get; set; } = default!;

    [MinLength(1)]
    [MaxLength(128)]
    public string LastName { get; set; } = default!;
    
    public Guid Id { get; set; }
}