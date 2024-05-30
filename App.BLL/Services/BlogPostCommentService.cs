using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.DTO;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using Base.Contracts.DAL;
using DAL.Contracts.Repositories;

namespace App.BLL.Services;

public class BlogPostCommentService : BaseEntityService<App.DAL.DTO.BlogPostComment, App.BLL.DTO.BlogPostComment, IBlogPostCommentRepository>,
    IBlogPostCommentService
{
    private readonly IMapper _mapper;

    public BlogPostCommentService(IAppUnitOfWork uoW, IBlogPostCommentRepository repository, IMapper mapper) : base(uoW, repository, new BllDalMapper<BlogPostComment, DTO.BlogPostComment>(mapper))
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }



    public async Task<IEnumerable<DTO.BlogPostComment>> GetAllBlogPostCommentsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetAllBlogPostCommentsIncludingAsync();
        return result.Select(rating => _mapper.Map<DTO.BlogPostComment>(rating));
    }
}