using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace Domain.App;

public class VeterinaryPractice: BaseEntityId
{
    [MaxLength(128)]
    public string? VeterinaryPracticeName { get; set; } = default!;
    public string? Location { get; set; } = default!;
    public string? PhoneNr { get; set; } = default!;
    public int RegistrationNr { get; set; }
    public ICollection<HealthRecord>? HealthRecords { get; set; }
    public ICollection<VeterinaryPracticeRating>? VeterinaryPracticeRatings { get; set; }
}