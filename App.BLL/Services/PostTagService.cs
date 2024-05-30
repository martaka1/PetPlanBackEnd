using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.DAL.DTO;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using DAL.Contracts.Repositories;
using PostTag = App.BLL.DTO.PostTags;

namespace App.BLL.Services;

public class PostTagService :
    BaseEntityService<App.DAL.DTO.PostTags, PostTag, IPostTagRepository>,
    IPostTagService
{
    private readonly IMapper _mapper;

    public PostTagService(IAppUnitOfWork uoW, IPostTagRepository repository, IMapper mapper) : base(uoW,
        repository, new BllDalMapper<App.DAL.DTO.PostTags, PostTag>(mapper))
    {
        _mapper = mapper;
    }


    public async Task<IEnumerable<PostTag>> GetAllPostTagIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetAllPostTagIncludingAsync();
        return result.Select(rating => _mapper.Map<DTO.PostTags>(rating));
    }
    
    public async Task<PostTag> AddNewPostTag(PostTag postTag)
    {
        if (postTag.TagId == default)
        {
            postTag.Tag = null;
        }

        if (postTag.BlogPostId == default)
        {
            postTag.BlogPost = null;
        }
        
        var result = Repository.Add(_mapper.Map<App.DAL.DTO.PostTags>(postTag));

        return _mapper.Map<App.BLL.DTO.PostTags>(result);
    }
}