using Base.Contracts.DAL;
using Domain.App;
using PostTags = App.DAL.DTO.PostTags;

namespace DAL.Contracts.Repositories;

public interface IPostTagRepository: IEntityRepository<PostTags>, IPostTagRepositoryCustom<PostTags>
{
    // define your custom methods here
}
public interface IPostTagRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllPostTagIncludingAsync(Guid userId = default, bool noTracking = true);

}