using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using DAL.Contracts.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using DALDTO = App.DAL.DTO;

namespace DAL.App.EF.Repositories;

public class BlogPostRepository : BaseEntityRepository<BlogPost, DALDTO.BlogPost, AppDbContext>,
    IBlogPostRepository
{
    private readonly IMapper _mapper;
    public BlogPostRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<BlogPost, DALDTO.BlogPost>(mapper))
    {
        _mapper = mapper;
    }
    

    public async Task<IEnumerable<DALDTO.BlogPost>> GetAllBlogPostsIncludedAsync()
    {
        var x = await CreateQuery()
            .Include(bloggPost=>bloggPost.AppUser)
            .Include(blogPost=>blogPost.Tags)!
            .ThenInclude(tags=>tags.Tag)
            .Include(blogPost=>blogPost.BlogPostComments)!
            .ThenInclude(comment=>comment.User)
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<DALDTO.BlogPost>>(x)
            : Enumerable.Empty<DALDTO.BlogPost>();
    }

    public async Task<IEnumerable<DALDTO.BlogPost>> GetWithoutCollectionBlogPostIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var x = await CreateQuery()
            .Include(bloggPost=>bloggPost.AppUser)
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<DALDTO.BlogPost>>(x)
            : Enumerable.Empty<DALDTO.BlogPost>();
    }
}