using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1_0;

public class VeterinaryPracticeWithoutCollection
{
    public Guid Id { get; set; }
    
    [MaxLength(128)]
    public string VeterinaryPracticeName { get; set; } = default!;
    public string? Location { get; set; } = default!;
    public string? PhoneNr { get; set; } = default!;
    public int RegistrationNr { get; set; }
}