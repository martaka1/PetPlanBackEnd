using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Identity;
using Base.Contracts.Domain;
using Base.Domain;


namespace Domain.App;

public class Pet: BaseEntityId,IDomainAppUser<AppUser>
{
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    [MaxLength(128)]
    public string? PetName { get; set; } = default!;
    [MaxLength(128)]
    public string? Spices { get; set; } = default!;
    [MaxLength(128)]
    public string? Breed { get; set; } = default!;
    public DateTime? Dob { get; set; }
    public int? ChipNr { get; set; }
    public ICollection<HealthRecord>? HealthRecords { get; set; }
    public ICollection<Appointment>? Appointments { get; set; }
}