using System.ComponentModel.DataAnnotations;
using App.DTO.v1_0.Identity;
using Domain.App.HelperEnums;
using EAppointmentType = App.DTO.v1_0.HelperEnums.EAppointmentType;

namespace App.DTO.v1_0;

public class Appointment
{
    public Guid Id { get; set; }
    
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    public Guid PetId { get; set; }
    public PetWithoutCollection? Pet { get; set; }

    public DateTime Date { get; set; }
    public EAppointmentType Type { get; set; } = default!;
}