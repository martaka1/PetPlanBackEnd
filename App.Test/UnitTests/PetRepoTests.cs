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
    public class PetRepositoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly Guid _petId;
        private readonly Guid _userId;
        private readonly PetRepository _repository;

        public PetRepositoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            _ctx = new AppDbContext(optionsBuilder.Options);

            var dalMapperConf = new MapperConfiguration(cfg => cfg.AddProfile<DAL.EF.AutoMapperProfile>());
            _mapper = dalMapperConf.CreateMapper();

            _repository = new PetRepository(_ctx, _mapper);

            _petId = Guid.NewGuid();
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

            var pet = new Pet()
            {
                Id = _petId,
                PetName = "Test Pet",
                AppUserId = _userId,
                AppUser = appUser,
                HealthRecords = new List<HealthRecord>
                {
                    new HealthRecord
                    {
                        Id = Guid.NewGuid(),
                        Notes = "Test Health Record",
                        AppUserId = _userId,
                        AppUser = appUser
                    }
                },
                Appointments = null
            };
            _ctx.Pet.Add(pet);
            await _ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllSortedAsync_ShouldReturnSortedPets()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetAllSortedAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(_petId, result.First().Id);
        }

        [Fact]
        public async Task GetAllPetIncludingAsync_ShouldReturnPetsWithIncludedEntities()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetAllPetIncludingAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            var enumerable = result as App.DAL.DTO.Pet[] ?? result.ToArray();
            Assert.That(enumerable.Length, Is.EqualTo(1));
            var pet = enumerable.First();
            Assert.NotNull(pet.AppUser);
            Assert.NotNull(pet.HealthRecords);
            Assert.NotNull(pet.Appointments);
        }

        [Fact]
        public async Task GetWithoutCollectionPetIncludingAsync_ShouldReturnPetsExcludingCollections()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetWithoutCollectionPetIncludingAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            var enumerable = result as App.DAL.DTO.Pet[] ?? result.ToArray();
            Assert.That(enumerable.Length, Is.EqualTo(1));
            var pet = enumerable.First();
            Assert.NotNull(pet.AppUser);
            Assert.IsEmpty(pet.HealthRecords);
            Assert.IsEmpty(pet.Appointments);
        }
    }
}
