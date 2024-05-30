using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1_0;

public class PostTags
{
    public Guid Id { get; set; }
    public Guid TagId { get; set; }
    public BlogTagWithoutCollection? Tag { get; set; }
    
    public Guid BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; }

}