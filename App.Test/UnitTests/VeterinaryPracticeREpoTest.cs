using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace App.Test.UnitTests
{
    public class VeterinaryPracticeRepositoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly Guid _veterinaryPracticeId;
        private readonly Guid _userId;
        private readonly VeterinaryPracticeRepository _repository;

        public VeterinaryPracticeRepositoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            _ctx = new AppDbContext(optionsBuilder.Options);

            var dalMapperConf = new MapperConfiguration(cfg => cfg.AddProfile<DAL.EF.AutoMapperProfile>());
            _mapper = dalMapperConf.CreateMapper();

            _repository = new VeterinaryPracticeRepository(_ctx, _mapper);

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
                VeterinaryPracticeName = "Test Veterinary Practice",
                HealthRecords = null,
                VeterinaryPracticeRatings = null
            };
            _ctx.VeterinaryPractice.Add(veterinaryPractice);
            await _ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllSortedAsync_ShouldReturnSortedVeterinaryPractices()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetAllSortedAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(_veterinaryPracticeId));
        }

        [Fact]
        public async Task GetAllVeterinaryPracticeRatingsIncludingAsync_ShouldReturnVeterinaryPracticesWithIncludedEntities()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetAllVeterinaryPracticeRatingsIncludingAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            var enumerable = result as App.DAL.DTO.VeterinaryPractice[] ?? result.ToArray();
            Assert.That(enumerable.Length, Is.EqualTo(1));
            var veterinaryPractice = enumerable.First();
            Assert.NotNull(veterinaryPractice.HealthRecords);
            Assert.NotNull(veterinaryPractice.VeterinaryPracticeRatings);
        }

        [Fact]
        public async Task GetAllVeterinaryPracticeWithoutCollectionsIncludingAsync_ShouldReturnVeterinaryPracticesExcludingCollections()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetAllVeterinaryPracticeWithoutCollectionsIncludingAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            var enumerable = result as App.DAL.DTO.VeterinaryPractice[] ?? result.ToArray();
            Assert.That(enumerable.Length, Is.EqualTo(1));
            var veterinaryPractice = enumerable.First();
            Assert.IsEmpty(veterinaryPractice.HealthRecords);
            Assert.IsEmpty(veterinaryPractice.VeterinaryPracticeRatings);
        }
    }
}
