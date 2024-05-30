using App.BLL;
using App.BLL.Services;
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

namespace App.Test.UnitTests
{
    public class VeterinaryPracticeRatingTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly AppDbContext _ctx;
        private readonly IAppUnitOfWork _appUow;
        private readonly Guid _veterinaryPracticeRatingId;
        private readonly Guid _userId;
        private readonly Guid _veterinaryPracticeId;

        public VeterinaryPracticeRatingTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            _ctx = new AppDbContext(optionsBuilder.Options);

            var dalMapperConf = new MapperConfiguration(cfg => cfg.AddProfile<DAL.EF.AutoMapperProfile>());

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _appUow = new AppUOW(_ctx, dalMapperConf.CreateMapper());
            _veterinaryPracticeRatingId = Guid.NewGuid();
            _veterinaryPracticeId = Guid.NewGuid();
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

            var veterinaryPractice = new VeterinaryPractice()
            {
                Id = _veterinaryPracticeId,
                VeterinaryPracticeName = "Test Practice",
                RegistrationNr = 10000,
                PhoneNr = "776384624",
                Location = "Test 1"
            };
            _ctx.VeterinaryPractice.Add(veterinaryPractice);
        
            await _ctx.SaveChangesAsync();

            var veterinaryPracticeRating = new VeterinaryPracticeRating()
            {
                Id = _veterinaryPracticeRatingId,
                Comment = "Test Rating Practice",
                Rating = 3,
                VeterinaryPracticeId = _veterinaryPracticeId,
                UserId = _userId,
                User = appUser,
                VeterinaryPractice = veterinaryPractice
            };
            _ctx.VeterinaryPracticeRating.Add(veterinaryPracticeRating);

            await _ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task FirstOrDefaultAsyncTest()
        {
            // ARRANGE
            await SeedData();
            // ACT
            var veterinaryPracticeRating = await _appUow.VeterinaryPracticeRating.FirstOrDefaultAsync(_veterinaryPracticeRatingId);
            await _appUow.SaveChangesAsync();
      
            // ASSERT
            Assert.NotNull(veterinaryPracticeRating);
            Assert.That(_veterinaryPracticeRatingId, Is.EqualTo(veterinaryPracticeRating.Id));
        }

        [Fact]
        public async Task GetAllSortedAsyncTest()
        {
            // ARRANGE
            await SeedData();
            // ACT
            var veterinaryPracticeRatings = await _appUow.VeterinaryPracticeRating.GetAllAsync();
            await _appUow.SaveChangesAsync();
            // ASSERT
            Assert.NotNull(veterinaryPracticeRatings);
            Assert.That(veterinaryPracticeRatings.Count(), Is.EqualTo(1));
        }

        [Fact]
        public async Task GetAllVeterinaryPracticeRatingsIncludingAsyncTest()
        {
            // ARRANGE
            await SeedData();

            var repository = _appUow.VeterinaryPracticeRating;
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = mapperConfiguration.CreateMapper();
            var service = new VeterinaryPracticeRatingService(_appUow,repository, mapper);

            // ACT
            var result = await service.GetAllVeterinaryPracticeRatingsIncludingAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));
            var rating = result.First();
            Assert.That(rating.Comment, Is.EqualTo("Test Rating Practice"));
            Assert.That(rating.Rating, Is.EqualTo(3));
            Assert.That(rating.UserId, Is.EqualTo(_userId));
            Assert.That(rating.VeterinaryPracticeId, Is.EqualTo(_veterinaryPracticeId));
        }
    }
}
