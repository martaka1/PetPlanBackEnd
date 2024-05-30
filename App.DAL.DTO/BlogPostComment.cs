using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Contracts.Domain;
using Base.Domain;
using AppUser = App.DAL.DTO.Identity.AppUser;

namespace App.DAL.DTO;

public class BlogPostComment: IDomainEntityId
{
    [MaxLength(10240)] 
    public string Comment { get; set; } = default!;
    public Guid BlogPostId { get; set; }
    [DataType(DataType.Date)] 
    public DateTime PostedAt { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; } = default!;
    public Guid Id { get; set; }
}