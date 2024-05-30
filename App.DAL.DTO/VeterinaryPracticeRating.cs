using System.ComponentModel.DataAnnotations;
using App.DAL.DTO.Identity;
using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class VeterinaryPracticeRating:IDomainEntityId
{
    [MaxLength(10240)] 
    public string Comment { get; set; } = default!;
    public int Rating { get; set; }
    
    public Guid VeterinaryPracticeId { get; set; }
    public VeterinaryPractice? VeterinaryPractice { get; set; }

    [DataType(DataType.Date)] 
    public DateTime PostedAt { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; } = default!;
    public Guid Id { get; set; }
}