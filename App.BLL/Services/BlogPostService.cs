using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.DAL.DTO;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using DAL.Contracts.Repositories;
using BlogPost = App.BLL.DTO.BlogPost;

namespace App.BLL.Services;

public class BlogPostService :
    BaseEntityService<App.DAL.DTO.BlogPost, BlogPost, IBlogPostRepository>,
    IBlogPostService
{
    private readonly IMapper _mapper;

    public BlogPostService(IAppUnitOfWork uoW, IBlogPostRepository repository, IMapper mapper) : base(uoW,
        repository, new BllDalMapper<App.DAL.DTO.BlogPost, BlogPost>(mapper))
    {
        _mapper = mapper;
    }

    

    public async Task<IEnumerable<BlogPost>> GetAllBlogPostsIncludedAsync()
    {
        var blogPosts = await Repository.GetAllBlogPostsIncludedAsync();
        return blogPosts.Select(bp => _mapper.Map<BlogPost>(bp));

    }

    public async Task<IEnumerable<BlogPost>> GetWithoutCollectionBlogPostIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var blogPosts = await Repository.GetWithoutCollectionBlogPostIncludingAsync();
        return blogPosts.Select(bp => _mapper.Map<BlogPost>(bp));
    }
}