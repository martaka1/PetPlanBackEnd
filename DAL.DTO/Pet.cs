using Domain.Base.Contracts.Domain.Base;

namespace DAL.DTO;

public class Pet: IDomainEntityId
{
    public Guid Id { get; set; }
    
    public string PetName { get; set; } = default!;
}
