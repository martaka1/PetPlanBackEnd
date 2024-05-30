using System.ComponentModel.DataAnnotations;
using App.BLL.DTO.Identity;
using Base.Contracts.Domain;

namespace App.BLL.DTO;

public class BlogPost: IDomainEntityId
{
    public Guid Id { get; set; }
    
    [MaxLength(128)]
    [Display(ResourceType = typeof(App.Resources.Domain.BlogPost), Name = nameof(Title))]
    public string Title { get; set; } = default!;
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    public string Content { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public DateTime Date { get; set; }

    public ICollection<PostTags>? Tags { get; set; }
    public ICollection<BlogPostComment>? Comments { get; set; }
}