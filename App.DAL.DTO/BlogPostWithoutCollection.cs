using System.ComponentModel.DataAnnotations;
using App.DAL.DTO.Identity;
using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class BlogPostWithoutCollection: IDomainEntityId
{
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    [MaxLength(128)]
    public string? Title { get; set; } = default!;
    [MaxLength(10248)]
    public string? Content { get; set; } = default!;
    [MaxLength(128)]
    public string? Summary { get; set; } = default!;
    [DataType(DataType.Date)] 
    public DateTime Date { get; set; }

    public Guid Id { get; set; }
}