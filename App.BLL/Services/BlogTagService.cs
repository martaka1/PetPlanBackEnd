using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.DAL.DTO;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using DAL.Contracts.Repositories;
using BlogTag = App.BLL.DTO.BlogTag;

namespace App.BLL.Services;

public class BlogTagService :
    BaseEntityService<App.DAL.DTO.BlogPostTag, BlogTag, IBlogTagRepository>,
    IBlogTagService
{
    private readonly IMapper _mapper;

    public BlogTagService(IAppUnitOfWork uoW, IBlogTagRepository repository, IMapper mapper) : base(uoW,
        repository, new BllDalMapper<App.DAL.DTO.BlogPostTag, BlogTag>(mapper))
    {
        _mapper = mapper;
    }


    public Task<IEnumerable<BlogTag>> GetAllSortedAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<BlogTag>> GetAllBlogTagsIncludedAsync()
    {
        var result = await Repository.GetAllBlogTagsIncludedAsync();
        return result.Select(blogPostTag => _mapper.Map<DTO.BlogTag>(blogPostTag));
        
    }
}