using System.ComponentModel.DataAnnotations;
using App.DTO.v1_0.HelperEnums;

namespace App.DTO.v1_0;

public class ExcerciseWithoutCollection
{
    public Guid Id { get; set; }
    
    public Guid AppUserId { get; set; }
    
    [MaxLength(128)]
    public string? Name { get; set; } = default!;
    [MaxLength(10248)]
    public string? Description { get; set; } = default!;
    public EExcerciseLevel Level { get; set; }
    public EExcerciseType Type { get; set; }
}