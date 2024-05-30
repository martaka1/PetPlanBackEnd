using System;
using System.Linq;
using System.Threading.Tasks;
using App.DAL.EF;
using App.Domain.Identity;
using AutoMapper;
using DAL.App.EF;
using DAL.App.EF.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;
using Assert = NUnit.Framework.Assert;

namespace App.Test.UnitTests
{
    public class IdentityRepoTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly AppDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly IdentityRepo _repository;

        public IdentityRepoTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _ctx = new AppDbContext(options);

            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<DAL.EF.AutoMapperProfile>());
            _mapper = mapperConfiguration.CreateMapper();

            _repository = new IdentityRepo(_ctx, _mapper);
        }

        [Fact]
        public async Task IsValid_WithValidToken_ShouldReturnTrue()
        {
            // Arrange
            var validToken = "validToken";
            var expirationDateTime = DateTime.Now.AddDays(1);
            var refreshToken = new AppRefreshToken
            {
                Id = Guid.NewGuid(),
                RefreshToken = validToken,
                ExpirationDT = expirationDateTime
            };
            await _ctx.RefreshTokens.AddAsync(refreshToken);
            await _ctx.SaveChangesAsync();

            // Act
            var result = await _repository.isValid(validToken);

            // Assert
            Assert.IsTrue(result);
        }

        [Fact]
        public async Task IsValid_WithInvalidToken_ShouldReturnFalse()
        {
            // Arrange
            var invalidToken = "invalidToken";

            // Act
            var result = await _repository.isValid(invalidToken);

            // Assert
            Assert.IsFalse(result);
        }
        
    }
}
