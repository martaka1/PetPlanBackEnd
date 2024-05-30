using System.ComponentModel.DataAnnotations;
using App.DAL.DTO.Identity;
using Base.Contracts.Domain;
using Domain.App.HelperEnums;
using EExcerciseLevel = App.DAL.DTO.HelperEnums.EExcerciseLevel;
using EExcerciseType = App.DAL.DTO.HelperEnums.EExcerciseType;

namespace App.DAL.DTO;

public class ExcerciseWithoutCollection: IDomainEntityId
{
    public Guid Id { get; set; }
    
    public Guid AppUserId { get; set; }
    public AppUser? Appuser { get; set; }
    [MaxLength(128)]
    public string? Name { get; set; } = default!;
    [MaxLength(10248)]
    public string? Description { get; set; } = default!;
    public EExcerciseLevel Level { get; set; }
    public EExcerciseType Type { get; set; }
}