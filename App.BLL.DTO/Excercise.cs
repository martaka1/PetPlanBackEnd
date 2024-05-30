using System.ComponentModel.DataAnnotations;
using App.BLL.DTO.Identity;
using Base.Contracts.Domain;
using Domain.App.HelperEnums;
using EExcerciseLevel = App.BLL.DTO.HelperEnums.EExcerciseLevel;
using EExcerciseType = App.BLL.DTO.HelperEnums.EExcerciseType;

namespace App.BLL.DTO;

public class Excercise: IDomainEntityId
{
    public Guid Id { get; set; }
    
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    [MaxLength(128)]
    public string? Name { get; set; } = default!;
    [MaxLength(10248)]
    public string? Description { get; set; } = default!;
    public EExcerciseLevel Level { get; set; }
    public EExcerciseType Type { get; set; }
    public ICollection<ExcerciseInTraining>? Trainings { get; set; }
    public ICollection<ExcerciseRating>? Ratings { get; set; }
}