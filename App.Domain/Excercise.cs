using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Contracts.Domain;
using Base.Domain;
using Domain.App.HelperEnums;


namespace Domain.App;

public class Excercise : BaseEntityId, IDomainAppUser<AppUser>
{
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