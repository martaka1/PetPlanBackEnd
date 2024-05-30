using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class PetWithoutCollection: IDomainEntityId
{
    public Guid Id { get; set; }
    public Guid AppUserId { get; set; }
    
    public string PetName { get; set; } = default!;
    public string Spices { get; set; } = default!;
    public string Breed { get; set; } = default!;
    public DateTime Dob { get; set; }
    public int ChipNr { get; set; }

}