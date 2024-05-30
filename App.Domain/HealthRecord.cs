using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;


namespace Domain.App;

public class HealthRecord:BaseEntityId
{
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    
    public Guid VeterinaryPracticeId { get; set; }
    public VeterinaryPractice? VeterinaryPractice { get; set; }
    
    public DateTime Date { get; set; }
    public int Weight { get; set; }
    [MaxLength(1024)]
    public string? Notes { get; set; }
    public string? Name { get; set; }
}