using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace App.BLL.DTO;

public class PostTags: IDomainEntityId
{
    public Guid Id { get; set; }
    
    public Guid TagId { get; set; }
    public BlogTag? Tag { get; set; }
    public Guid BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; }
}