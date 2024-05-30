using Base.Domain;

namespace Domain.App;

public class PostTags:BaseEntityId
{
    public Guid TagId { get; set; }
    public BlogTag? Tag { get; set; }
    
    public Guid BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; }
}