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
    public class HealthRecordRepositoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly Guid _healthRecordId;
        private readonly Guid _userId;
        private readonly HealthRecordRepository _repository;

        public HealthRecordRepositoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            _ctx = new AppDbContext(optionsBuilder.Options);

            var dalMapperConf = new MapperConfiguration(cfg => cfg.AddProfile<DAL.EF.AutoMapperProfile>());
            _mapper = dalMapperConf.CreateMapper();

            _repository = new HealthRecordRepository(_ctx, _mapper);

            _healthRecordId = Guid.NewGuid();
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

            var healthRecord = new HealthRecord()
            {
                Id = _healthRecordId,
                AppUserId = _userId,
                AppUser = appUser,
                Pet = new Pet
                {
                    Id = Guid.NewGuid(),
                    PetName = "Test Pet"
                },
                VeterinaryPractice = new VeterinaryPractice
                {
                    Id = Guid.NewGuid(),
                    VeterinaryPracticeName = "Test Vet"
                }
            };
            _ctx.HealthRecord.Add(healthRecord);
            await _ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllSortedAsync_ShouldReturnSortedHealthRecords()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetAllSortedAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(_healthRecordId, result.First().Id);
        }

        [Fact]
        public async Task GetAllHealthRecordIncludingAsync_ShouldReturnHealthRecordsWithIncludedEntities()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetAllHealthRecordIncludingAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            var enumerable = result as App.DAL.DTO.HealthRecord[] ?? result.ToArray();
            Assert.AreEqual(1, enumerable.Length);
            var healthRecord = enumerable.First();
            Assert.NotNull(healthRecord.AppUser);
            Assert.NotNull(healthRecord.Pet);
            Assert.NotNull(healthRecord.VeterinaryPractice);
        }

        [Fact]
        public async Task GetAllHealthRecordWithoutCollectionIncludingAsync_ShouldReturnHealthRecordsExcludingCollections()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetAllHealthRecordwithoutcollectionIncludingAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            var enumerable = result as App.DAL.DTO.HealthRecord[] ?? result.ToArray();
            Assert.AreEqual(1, enumerable.Length);
            var healthRecord = enumerable.First();
            Assert.NotNull(healthRecord.AppUser);
            Assert.Null(healthRecord.Pet);
            Assert.Null(healthRecord.VeterinaryPractice);
        }
    }
}
