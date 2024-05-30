using App.Contracts.DAL;
using App.DAL.DTO;
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
using BlogPost = Domain.App.BlogPost;

namespace App.Test.UnitTests
{
    public class BlogTagTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly AppDbContext _ctx;
        private readonly IAppUnitOfWork _appUow;
        private readonly Guid _tagId;
        private readonly Guid _userId;
        private readonly IMapper _mapper;

        public BlogTagTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            _ctx = new AppDbContext(optionsBuilder.Options);

            var dalMapperConf = new MapperConfiguration(cfg => cfg.AddProfile<DAL.EF.AutoMapperProfile>());
            _mapper = dalMapperConf.CreateMapper();

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _appUow = new AppUOW(_ctx, _mapper);
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
                Name = "Test",
                BlogPosts = null
            };
            _ctx.BlogTag.Add(newTag);
            await _ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task FirstOrDefaultAsyncTest()
        {
            // ARRANGE
            await SeedData();
            // ACT
            var blogTag = await _appUow.BlogTag.FirstOrDefaultAsync(_tagId);
            await _appUow.SaveChangesAsync();

            // Assert
            Assert.NotNull(blogTag);
            Assert.That(_tagId, Is.EqualTo(blogTag.Id));
        }

        [Fact]
        public async Task GetAllSortedAsyncTest()
        {
            // ARRANGE
            await SeedData();
            // ACT
            var blogTags = await _appUow.BlogTag.GetAllAsync();
            await _appUow.SaveChangesAsync();
            // ASSERT
            Assert.NotNull(blogTags);
            Assert.That(blogTags.Count(), Is.EqualTo(1));
        }

        [Fact]
        public async Task GetAllBlogTagsIncludedAsync_ShouldReturnBlogTagsWithIncludedEntities()
        {
            // ARRANGE
            await SeedData();
            var repository = new BlogTagRepository(_ctx, _mapper);

            // ACT
            var result = await repository.GetAllBlogTagsIncludedAsync();

            // ASSERT
            Assert.NotNull(result);
            var enumerable = result as App.DAL.DTO.BlogPostTag[] ?? result.ToArray();
            Assert.That(enumerable.Length, Is.EqualTo(1));
            Assert.Null(enumerable.First().Posts);
        }

        [Fact]
        public async Task GetAllSortedAsync_ShouldReturnSortedBlogTags()
        {
            // ARRANGE
            await SeedData();
            var repository = new BlogTagRepository(_ctx, _mapper);

            // ACT
            var result = await repository.GetAllSortedAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));
        }
    }
}
