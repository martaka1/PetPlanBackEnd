using System.ComponentModel.DataAnnotations;
using App.DTO.v1_0.Identity;

namespace App.DTO.v1_0;

public class BlogPost
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = default!;
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    public string Content { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public DateTime Date { get; set; }

    public ICollection<PostTags>? Tags { get; set; }
    public ICollection<BlogPostComment>? BlogPostComments { get; set; }
}