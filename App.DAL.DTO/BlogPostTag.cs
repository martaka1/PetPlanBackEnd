using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class BlogPostTag: IDomainEntityId
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = default!;
    public ICollection<PostTags>? Posts { get; set; }
}