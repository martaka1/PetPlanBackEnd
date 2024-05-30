using System.ComponentModel.DataAnnotations;
using App.DTO.v1_0.Identity;

namespace App.DTO.v1_0;

public class HomeTrainingRating
{
    public Guid Id { get; set; }
    [MaxLength(10240)] 
    public string Comment { get; set; } = default!;
    public int Rating { get; set; }
    
    public Guid HomeTrainingId { get; set; }
    public HomeTraining? HomeTraining { get; set; }

    public DateTime PostedAt { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; } = default!;
}
