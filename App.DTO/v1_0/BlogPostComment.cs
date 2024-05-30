using System.ComponentModel.DataAnnotations;
using App.DTO.v1_0.Identity;

namespace App.DTO.v1_0;

public class BlogPostComment
{
    
    public Guid Id { get; set; }
    public string Comment { get; set; } = default!;
    public Guid BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; } = default!;
    public DateTime PostedAt { get; set; }

    public Guid UserId { get; set; }
    public AppUser? User { get; set; } = default!;
}