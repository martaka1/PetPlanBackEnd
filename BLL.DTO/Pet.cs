using System.ComponentModel.DataAnnotations;

namespace BLL.DTO;


public class Pet: IDomainEntityId
{
    public Guid Id { get; set; }
    
    [MaxLength(128)]
    [Display(ResourceType = typeof(Resources.Domain.Contest), Name = nameof(PetName))]
    public string PetName { get; set; } = default!;
}
