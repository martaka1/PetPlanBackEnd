using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using AppUser = App.DTO.v1_0.Identity.AppUser;

namespace App.DTO.v1_0;

public class Pet
{
    public Guid Id { get; set; }
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    public string PetName { get; set; } = default!;
    public string Spices { get; set; } = default!;
    public string Breed { get; set; } = default!;
    public DateTime? Dob { get; set; }
    public int? ChipNr { get; set; }
    public ICollection<HealthRecord>? HealthRecords { get; set; } 
    public ICollection<Appointment>? Appointments { get; set; }

}