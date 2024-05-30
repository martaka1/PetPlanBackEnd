using Base.Contracts.DAL;

namespace DAL.Contracts.Repositories;

public interface IBlogPostCommentRepository : IEntityRepository<App.DAL.DTO.BlogPostComment>, IBlogPostCommentRepositoryCustom<App.DAL.DTO.BlogPostComment>
{
    
}


public interface IBlogPostCommentRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllBlogPostCommentsIncludingAsync(Guid userId = default, bool noTracking = true);
}