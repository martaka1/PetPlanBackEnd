using System.ComponentModel.DataAnnotations;
using App.DAL.DTO.Identity;
using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class Pet: IDomainEntityId
{
    public Guid Id { get; set; }
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    public string PetName { get; set; } = default!;
    public string Spices { get; set; } = default!;
    public string Breed { get; set; } = default!;
    public DateTime Dob { get; set; }
    public int ChipNr { get; set; }
    public ICollection<HealthRecord>? HealthRecords;
    public ICollection<Appointment>? Appointments { get; set; }

}