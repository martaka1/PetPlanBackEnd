using System;
using DAL.App.EF;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assert = Xunit.Assert;

namespace App.Test.UnitTests
{
    public class AppDbContextFactoryTests
    {
        [Fact]
        public void CreateDbContext_ShouldReturnDbContextWithExpectedOptions()
        {
            // ARRANGE
            var factory = new AppDbContextFactory();

            // ACT
            var dbContext = factory.CreateDbContext(Array.Empty<string>());

            // ASSERT
            Assert.NotNull(dbContext);

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=7890;Database=petplan;Username=postgres;Password=postgres");

            var expectedOptions = "Host=localhost;Port=7890;Database=petplan;Username=postgres;Password=postgres";
            var actualOptions = dbContext.Database.GetDbConnection().ConnectionString;

            Assert.Equal(expectedOptions.ToString(), actualOptions);
        }
    }
}