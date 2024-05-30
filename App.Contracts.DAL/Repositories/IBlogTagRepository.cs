using App.DAL.DTO;
using Base.Contracts.DAL;
using Domain.App;

namespace DAL.Contracts.Repositories;

public interface IBlogTagRepository : IEntityRepository<BlogPostTag>, IBlogTagRepositoryCustom<BlogPostTag>
{
    
}
public interface IBlogTagRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllSortedAsync(Guid userId);
    Task<IEnumerable<TEntity>> GetAllBlogTagsIncludedAsync();

}