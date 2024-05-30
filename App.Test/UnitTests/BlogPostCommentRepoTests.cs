using App.Contracts.DAL;
using App.Domain.Identity;
using AutoMapper;
using DAL.App.EF;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using Assert = NUnit.Framework.Assert;

namespace App.Test.UnitTests;

public class BlogPostCommentTEsts
{
  
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly AppDbContext _ctx;
    private readonly IAppUnitOfWork _appUow;
    private readonly Guid _commentId;
    private readonly Guid _userId;
    private readonly Guid _postId;


    public BlogPostCommentTEsts(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;


        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _ctx = new AppDbContext(optionsBuilder.Options);


        var dalMapperConf = new MapperConfiguration(cfg => cfg.AddProfile<DAL.EF.AutoMapperProfile>());


        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _appUow = new AppUOW(_ctx, dalMapperConf.CreateMapper());
        _commentId = Guid.NewGuid();
        _postId = Guid.NewGuid();
        _userId = Guid.NewGuid();


    }

    internal async Task SeedData()
    {
        var appUser = new AppUser()
        {
            Id = _userId,
            Email = "admin@eesti.ee",
            UserName = "admin@eesti.ee",
            FirstName = "Admin",
            LastName = "Eesti"
        };
        _ctx.Users.Add(appUser);
        await _ctx.SaveChangesAsync();
        var newBlogPost = new BlogPost()
        {
            Id = _postId,
            Title = "Test",
            Summary = "This is a testing blogPost",
            Content = "Test test test test test test test test.",
        };
        _ctx.BlogPost.Add(newBlogPost);
        
        await _ctx.SaveChangesAsync();
        var comment = new BlogPostComment()
        {
            Id = _commentId,
            Comment = "Test Rating Practice",
            BlogPostId = _postId,
            UserId = _userId,
            BlogPost = newBlogPost,
            User = appUser,
        };
        _ctx.BlogPostComment.Add(comment);

        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllBlogPostCommentsIncludingAsync_ReturnsAllCommentsWithUser()
    {
        // ARRANGE
        await SeedData();

        // ACT
        var comments = await _appUow.BlogPostComment.GetAllBlogPostCommentsIncludingAsync();

        // ASSERT
        Assert.NotNull(comments);
        Assert.That(comments.Count(), Is.EqualTo(1)); // Assuming only one comment is seeded
        Assert.NotNull(comments.First().User); // Verify that the user is included
        Assert.NotNull(comments.First().Comment);
    }

    [Fact]
    public async Task FirstOrDefaultAsyncTest()
    {
        // ARRANGE
        await SeedData();

        // ACT
        var blogPostComment = await _appUow.BlogPostComment.FirstOrDefaultAsync(_commentId);

        // ASSERT
        Assert.NotNull(blogPostComment);
        Assert.That(blogPostComment.Id, Is.EqualTo(_commentId));
    }

    [Fact]
    public async Task GetAllSortedAsyncTest()
    {
        // ARRANGE
        await SeedData();

        // ACT
        var blogPostComments = await _appUow.BlogPostComment.GetAllAsync();

        // ASSERT
        Assert.NotNull(blogPostComments);
        Assert.That(blogPostComments.Count(), Is.EqualTo(1));
    }
}
