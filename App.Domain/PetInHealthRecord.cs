using Base.Domain;

namespace Domain.App;

public class PetInHealthRecord:BaseEntityId
{
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    
    public Guid HealthRecordId { get; set; }
    public HealthRecord? HealthRecord { get; set; }
}