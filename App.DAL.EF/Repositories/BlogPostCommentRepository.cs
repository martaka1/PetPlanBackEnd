using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using DAL.Contracts.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using DALDTO = App.DAL.DTO;

namespace DAL.App.EF.Repositories;

public class BlogPostCommentRepository :
    BaseEntityRepository<BlogPostComment,  DALDTO.BlogPostComment, AppDbContext>, IBlogPostCommentRepository
{
    private readonly IMapper _mapper;
    public BlogPostCommentRepository(AppDbContext dbContext, IMapper mapper) : 
        base(dbContext, new DalDomainMapper<BlogPostComment, DALDTO.BlogPostComment>(mapper))
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<DALDTO.BlogPostComment>> GetAllBlogPostCommentsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await CreateQuery()
            .Include(comment=>comment.User)
            .ToListAsync();
        var mapped = result.Select(de => Mapper.Map(de));
        return mapped != null
            ? _mapper.Map<IEnumerable<DALDTO.BlogPostComment>>(result)
            : Enumerable.Empty<DALDTO.BlogPostComment>();
    }
}