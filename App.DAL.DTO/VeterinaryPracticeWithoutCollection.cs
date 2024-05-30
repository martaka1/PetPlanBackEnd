using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class VeterinaryPracticeWithoutCollection: IDomainEntityId
{
    public Guid Id { get; set; }
    
    public string VeterinaryPracticeName { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string PhoneNr { get; set; } = default!;
    public int RegistrationNr { get; set; }
}