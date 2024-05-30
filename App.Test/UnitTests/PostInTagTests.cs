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

public class PostInTagTests
{
  
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly AppDbContext _ctx;
    private readonly IAppUnitOfWork _appUow;
    private readonly Guid _postInTagId;
    private readonly Guid _userId;
    private readonly Guid _emptyBlogPostId;
    private readonly Guid _postId;
    private readonly Guid _tagId;

    public PostInTagTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;


        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _ctx = new AppDbContext(optionsBuilder.Options);


        var dalMapperConf = new MapperConfiguration(cfg => cfg.AddProfile<DAL.EF.AutoMapperProfile>());


        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _appUow = new AppUOW(_ctx, dalMapperConf.CreateMapper());
        _postInTagId = Guid.NewGuid();
        _emptyBlogPostId = Guid.NewGuid();
        _postId = Guid.NewGuid();
        _tagId = Guid.NewGuid();
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
        
        var newTag = new BlogTag()
        {
            Id = _tagId,
            Name="Test"
        };
        _ctx.BlogTag.Add(newTag);
        
        var newBlogPost = new BlogPost()
        {
            Id = _postId,
            Title = "Test",
            Summary = "This is a testing blogPost",
            Content = "Test test test test test test test test."
        };
        _ctx.BlogPost.Add(newBlogPost);
        
        await _ctx.SaveChangesAsync();
        var postTags = new PostTags()
        {
            Id = _postInTagId,
            BlogPostId= _postId,
            TagId = _tagId,
            BlogPost = newBlogPost,
            Tag = newTag
        };
        _ctx.PostTags.Add(postTags);

        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task FirstOrDefaultAsyncTest()
    {
        // ARRANGE
        await SeedData();
        // ACT
        var blogPost = await _appUow.PostTag.FirstOrDefaultAsync(_postInTagId);
        await _appUow.SaveChangesAsync();
      
        // Assert
        Assert.NotNull(blogPost);
        Assert.That(_postInTagId, Is.EqualTo(blogPost.Id));
    }
    [Fact]
    public async Task GetAllSortedAsyncTest()
    {
        // ARRANGE
        await SeedData();
        // ACT
        var blogposts = await _appUow.PostTag.GetAllAsync();
        await _appUow.SaveChangesAsync();
        // ASSERT
        Assert.NotNull(blogposts);
        Assert.That(blogposts.Count(), Is.EqualTo(1));
    }
}