using App.BLL.DTO;
using Base.Contracts.DAL;
using DAL.Contracts.Repositories;

namespace App.Contracts.BLL.Services;

public interface IPostTagService : IEntityRepository<PostTags>, IPostTagRepositoryCustom<PostTags>
{
    public Task<App.BLL.DTO.PostTags> AddNewPostTag(App.BLL.DTO.PostTags postTag);
}