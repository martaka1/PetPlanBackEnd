using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Contracts.Domain;
using Base.Domain;
using Domain.App.HelperEnums;


namespace Domain.App;


public class Appointment: BaseEntityId, IDomainAppUser<AppUser>
{
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    public EAppointmentType Type { get; set; }
}