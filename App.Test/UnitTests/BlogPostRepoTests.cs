using App.Contracts.DAL;
using App.Domain.Identity;
using AutoMapper;
using DAL.App.EF;
using DAL.App.EF.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using Assert = NUnit.Framework.Assert;





namespace App.Test.UnitTests;

public class BlogPostTests
{
  
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly AppDbContext _ctx;
    private readonly IAppUnitOfWork _appUow;
    private readonly Guid _blogPostId;
    private readonly Guid _userId;
    private readonly Guid _emptyBlogPostId;
    private readonly IMapper _mapper;

    public BlogPostTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;


        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _ctx = new AppDbContext(optionsBuilder.Options);
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DAL.EF.AutoMapperProfile>();
            // Add any additional mappings if needed
        });
        _mapper = mapperConfig.CreateMapper();

        var dalMapperConf = new MapperConfiguration(cfg => cfg.AddProfile<DAL.EF.AutoMapperProfile>());


        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _appUow = new AppUOW(_ctx, dalMapperConf.CreateMapper());
        _blogPostId = Guid.NewGuid();
        _emptyBlogPostId = Guid.NewGuid();
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
        
        await _ctx.SaveChangesAsync();
        var newBlogPost = new BlogPost()
        {
            Id = _blogPostId,
            Title = "Test",
            Summary = "This is a testing blogPost",
            Content = "Test test test test test test test test.",
            Tags = null,
            AppUserId = _userId,
            AppUser = appUser,
            BlogPostComments = null
        };
        _ctx.BlogPost.Add(newBlogPost);

        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task FirstOrDefaultAsyncTest()
    {
        // ARRANGE
        await SeedData();
        // ACT
        var blogPost = await _appUow.BlogPost.FirstOrDefaultAsync(_blogPostId);
        await _appUow.SaveChangesAsync();
      
        // Assert
        Assert.NotNull(blogPost);
        Assert.That(_blogPostId, Is.EqualTo(blogPost.Id));
    }
    [Fact]
    public async Task GetAllSortedAsyncTest()
    {
        // ARRANGE
        await SeedData();
        // ACT
        var blogposts = await _appUow.BlogPost.GetAllAsync();
        await _appUow.SaveChangesAsync();
        // ASSERT
        Assert.NotNull(blogposts);
        Assert.That(blogposts.Count(), Is.EqualTo(1));
    }

    [Fact]
    public async Task GetAllBlogPostsIncludedAsync_ReturnsAllBlogPostsWithIncludedEntities()
    {
        // ARRANGE
        await SeedData();
        var repository = new BlogPostRepository(_ctx, _mapper);

        // ACT
        var blogPosts = await repository.GetAllBlogPostsIncludedAsync();

        // ASSERT
        Assert.NotNull(blogPosts);
        var enumerable = blogPosts as DAL.DTO.BlogPost[] ?? blogPosts.ToArray();
        Assert.That(enumerable.Count(), Is.EqualTo(1));
        var firstBlogPost = enumerable.FirstOrDefault();
        Assert.NotNull(firstBlogPost?.AppUser);
        Assert.NotNull(firstBlogPost?.Tags);
        Assert.NotNull(firstBlogPost?.BlogPostComments);
    }

    [Fact]
    public async Task GetWithoutCollectionBlogPostIncludingAsync_ReturnsBlogPostsExcludingCollections()
    {
        // ARRANGE
        await SeedData();
        var repository = new BlogPostRepository(_ctx, _mapper);

        // ACT
        var blogPosts = await repository.GetWithoutCollectionBlogPostIncludingAsync();

        // ASSERT
        Assert.NotNull(blogPosts);
        var enumerable = blogPosts.ToList();
        Assert.That(enumerable.Count(), Is.EqualTo(1));
        var firstBlogPost = enumerable.FirstOrDefault();
        Assert.NotNull(firstBlogPost?.AppUser);
        Assert.IsEmpty(firstBlogPost?.Tags);
        Assert.IsEmpty(firstBlogPost?.BlogPostComments);
    }
}