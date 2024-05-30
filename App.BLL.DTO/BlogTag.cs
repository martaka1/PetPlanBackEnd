using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace App.BLL.DTO;

public class BlogTag: IDomainEntityId
{
    public Guid Id { get; set; }
    
    [MaxLength(128)]
    [Display(ResourceType = typeof(App.Resources.BlogTag), Name = nameof(Name))]
    public string Name { get; set; } = default!;
    public ICollection<PostTags>? Posts { get; set; }
}