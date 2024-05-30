namespace App.DTO.v1_0;

public class PetWithoutCollection
{
    public Guid Id { get; set; }
    public Guid AppUserId { get; set; }
    
    public string PetName { get; set; } = default!;
    public string Spices { get; set; } = default!;
    public string Breed { get; set; } = default!;
    public DateTime Dob { get; set; }
    public int ChipNr { get; set; }
}