using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Domain;

namespace Domain.App;

public class BlogPostComment: BaseEntityId
{
    [MaxLength(10240)] 
    public string Comment { get; set; } = default!;
    public Guid BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; } = default!;
    [DataType(DataType.Date)] 
    public DateTime PostedAt { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; } = default!;
}