using System.ComponentModel.DataAnnotations;
using App.BLL.DTO.Identity;
using Base.Contracts.Domain;

namespace App.BLL.DTO;

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