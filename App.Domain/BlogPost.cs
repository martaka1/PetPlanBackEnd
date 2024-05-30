using System.ComponentModel.DataAnnotations;
using App.Domain.Identity;
using Base.Contracts.Domain;
using Base.Domain;

namespace Domain.App;

public class BlogPost: BaseEntityId, IDomainAppUser<AppUser>
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
    public ICollection<BlogPostComment>? BlogPostComments { get; set; }
    public ICollection<PostTags>? Tags { get; set; }
    
}