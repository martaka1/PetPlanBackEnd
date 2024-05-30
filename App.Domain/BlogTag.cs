using System.ComponentModel.DataAnnotations;
using Base.Domain;


namespace Domain.App;

public class BlogTag: BaseEntityId
{
    [MaxLength(128)]
    public string? Name { get; set; } = default!;

    public ICollection<PostTags>? BlogPosts { get; set; }
}