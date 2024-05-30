using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using DAL.Contracts.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using DALDTO = App.DAL.DTO;

namespace DAL.App.EF.Repositories;

public class PostTagsRepository : BaseEntityRepository<PostTags, DALDTO.PostTags, AppDbContext>,
    IPostTagRepository
{
    public PostTagsRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<PostTags, DALDTO.PostTags>(mapper))
    {
    }

    // implement your custom methods here
    public async Task<IEnumerable<DALDTO.PostTags>> GetAllSortedAsync(Guid userId)
    {
        var query = CreateQuery(userId);
        var res = await query.ToListAsync();
//        query = query.OrderBy(c => c.ContestName);
        
        return res.Select(e => Mapper.Map(e));
    }

    public async Task<IEnumerable<DALDTO.PostTags>> GetAllPostTagIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        throw new NotImplementedException();
    }
}