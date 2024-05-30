using System.ComponentModel.DataAnnotations;
using App.BLL.DTO.Identity;
using Base.Contracts.Domain;

namespace App.BLL.DTO;

public class Pet: IDomainEntityId
{
    public Guid Id { get; set; }
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    [MaxLength(128)]
    [Display(ResourceType = typeof(App.Resources.Domain.Pet), Name = nameof(PetName))]
    public string PetName { get; set; } = default!;
    public string Spices { get; set; } = default!;
    public string Breed { get; set; } = default!;
    public DateTime? Dob { get; set; }
    public int? ChipNr { get; set; }
    public ICollection<HealthRecord>? HealthRecords;
    public ICollection<Appointment>? Appointments { get; set; }

}