using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace Domain.App;

public class HomeTrainingRating:BaseEntityId
{
    [MaxLength(10240)] 
    public string Comment { get; set; } = default!;
    public int Rating { get; set; }
    
    public Guid HomeTrainingId { get; set; }
    public HomeTraining? HomeTraining { get; set; }

    [DataType(DataType.Date)] 
    public DateTime PostedAt { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; } = default!;
}