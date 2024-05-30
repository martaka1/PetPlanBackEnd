using Base.Contracts.DAL;
using Domain.App;
using BlogPost = App.DAL.DTO.BlogPost;

namespace DAL.Contracts.Repositories;

public interface IBlogPostRepository : IEntityRepository<BlogPost>, IBlogPostRepositoryCustom<BlogPost>
{
    // define your custom methods here
}
public interface IBlogPostRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllBlogPostsIncludedAsync();
    Task<IEnumerable<TEntity>> GetWithoutCollectionBlogPostIncludingAsync(Guid userId = default, bool noTracking = true);


}