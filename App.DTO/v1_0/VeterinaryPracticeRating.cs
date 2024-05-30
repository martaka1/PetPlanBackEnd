using System.ComponentModel.DataAnnotations;
using App.DTO.v1_0.Identity;

namespace App.DTO.v1_0;

public class VeterinaryPracticeRating
{
    [MaxLength(10240)] 
    public string Comment { get; set; } = default!;
    public int Rating { get; set; }
    
    public Guid VeterinaryPracticeId { get; set; }
    public VeterinaryPractice? VeterinaryPractice { get; set; }

    public DateTime PostedAt { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; } = default!;
    public Guid Id { get; set; }
}