using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace App.BLL.DTO;

public class VeterinaryPractice: IDomainEntityId
{
    public Guid Id { get; set; }
    public string VeterinaryPracticeName { get; set; } = default!;
    public string? Location { get; set; } = default!;
    public string? PhoneNr { get; set; } = default!;
    public int RegistrationNr { get; set; }
    public ICollection<HealthRecord>? HealthRecords { get; set; }
    public ICollection<VeterinaryPracticeRating>? VeterinaryPracticeRatings { get; set; }

}