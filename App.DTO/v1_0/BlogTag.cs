using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1_0;

public class BlogTag
{
    public Guid Id { get; set; }
    
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    public ICollection<PostTags>? Posts { get; set; }

}