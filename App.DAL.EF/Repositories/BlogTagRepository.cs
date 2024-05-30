using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using DAL.Contracts.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using DALDTO = App.DAL.DTO;

namespace DAL.App.EF.Repositories;

public class BlogTagRepository : BaseEntityRepository<BlogTag, DALDTO.BlogPostTag, AppDbContext>,
    IBlogTagRepository
{
    private readonly IMapper _mapper;
    public BlogTagRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<BlogTag, DALDTO.BlogPostTag>(mapper))
    {
        _mapper = mapper;
    }

    // implement your custom methods here
    public async Task<IEnumerable<DALDTO.BlogPostTag>> GetAllSortedAsync(Guid userId)
    {
        var query = CreateQuery(userId);
        var res = await query.ToListAsync();
//        query = query.OrderBy(c => c.ContestName);
        
        return res.Select(e => Mapper.Map(e));
    }

    public async Task<IEnumerable<DALDTO.BlogPostTag>> GetAllBlogTagsIncludedAsync()
    {
        var x = await CreateQuery()
            .Include(tag=>tag.BlogPosts)
            .ThenInclude(blogTags=>blogTags.BlogPost)
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<global::App.DAL.DTO.BlogPostTag>>(x)
            : Enumerable.Empty<DALDTO.BlogPostTag>();
    }
}