using System.ComponentModel.DataAnnotations;
using Domain.App.HelperEnums;
using EExcerciseLevel = App.DTO.v1_0.HelperEnums.EExcerciseLevel;
using EExcerciseType = App.DTO.v1_0.HelperEnums.EExcerciseType;

namespace App.DTO.v1_0;

public class Excercise
{
    public Guid Id { get; set; }
    
    public Guid AppUserId { get; set; }
    
    [MaxLength(128)]
    public string? Name { get; set; } = default!;
    [MaxLength(10248)]
    public string? Description { get; set; } = default!;
    public EExcerciseLevel Level { get; set; }
    public EExcerciseType Type { get; set; }
    public ICollection<ExcerciseInTraining>? Trainings { get; set; }
    public ICollection<ExcerciseRating>?ExcerciseRating { get; set; }

}