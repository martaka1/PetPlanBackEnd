using System.ComponentModel.DataAnnotations;
using App.DAL.DTO.Identity;
using Base.Contracts.Domain;
using Domain.App.HelperEnums;
using EAppointmentType = App.DAL.DTO.HelperEnums.EAppointmentType;

namespace App.DAL.DTO;

public class Appointment: IDomainEntityId
{
    public Guid Id { get; set; }
    
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    public Guid PetId { get; set; }
    public PetWithoutCollection? Pet { get; set; }

    public DateTime Date { get; set; }
    public EAppointmentType Type { get; set; } = default!;
}