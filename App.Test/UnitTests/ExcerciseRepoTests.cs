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
    public class ExcerciseRepositoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly Guid _excerciseId;
        private readonly Guid _userId;
        private readonly ExcerciseRepository _repository;

        public ExcerciseRepositoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            _ctx = new AppDbContext(optionsBuilder.Options);

            var dalMapperConf = new MapperConfiguration(cfg => cfg.AddProfile<DAL.EF.AutoMapperProfile>());
            _mapper = dalMapperConf.CreateMapper();

            _repository = new ExcerciseRepository(_ctx, _mapper);

            _excerciseId = Guid.NewGuid();
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

            var excercise = new Excercise()
            {
                Id = _excerciseId,
                Name = "Test Excercise",
                AppUserId = _userId,
                AppUser = appUser,
                Ratings = null,
                Trainings = null
            };
            _ctx.Excercise.Add(excercise);
            await _ctx.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllSortedAsync_ShouldReturnSortedExcercises()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetAllSortedAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Test Excercise"));
        }
        
        [Fact]
        public async Task GetAllExcercisesIncludingAsync_ShouldReturnExcercisesWithIncludedEntities()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetAllExcercisesIncludingAsync(_userId);

            // ASSERT
            Assert.NotNull(result);
            var enumerable = result as App.DAL.DTO.Excercise[] ?? result.ToArray();
            Assert.That(enumerable.Length, Is.EqualTo(1));
            var excercise = enumerable.First();
            Assert.NotNull(excercise.Appuser);
            Assert.Null(excercise.ExcerciseRating);
        }

        [Fact]
        public async Task GetAllExcercisesIncludingAsync_WithDefaultUserId_ShouldReturnExcercises()
        {
            // ARRANGE
            await SeedData();

            // ACT
            var result = await _repository.GetAllExcercisesIncludingAsync();

            // ASSERT
            Assert.NotNull(result);
            var enumerable = result as App.DAL.DTO.Excercise[] ?? result.ToArray();
            Assert.That(enumerable.Length, Is.EqualTo(1));
            var excercise = enumerable.First();
            Assert.NotNull(excercise.Appuser);
            Assert.Null(excercise.ExcerciseRating);
        }
    }
}
