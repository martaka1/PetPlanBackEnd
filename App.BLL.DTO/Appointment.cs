using System.ComponentModel.DataAnnotations;
using App.BLL.DTO.Identity;
using Base.Contracts.Domain;
using Domain.App.HelperEnums;
using EAppointmentType = App.BLL.DTO.HelperEnums.EAppointmentType;

namespace App.BLL.DTO;

public class Appointment: IDomainEntityId
{
    public Guid Id { get; set; }
    
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    public DateTime Date { get; set; }
    public EAppointmentType Type { get; set; } = default!;
}