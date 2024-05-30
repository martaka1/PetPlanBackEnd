using System.ComponentModel.DataAnnotations;
using App.BLL.DTO;
using App.BLL.DTO.Identity;
using Base.Contracts.Domain;

namespace App.BLL.DTO;

public class HomeTrainingRating:IDomainEntityId
{
    public Guid Id { get; set; }
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