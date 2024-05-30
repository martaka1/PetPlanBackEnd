using App.Contracts.DAL;
using App.Domain.Identity;
using AutoMapper;
using DAL.App.EF;
using Domain.App;
using Domain.App.HelperEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using Assert = NUnit.Framework.Assert;





namespace App.Test.UnitTests;

public class ExcerciseRatingRepoTests
{
  
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly AppDbContext _ctx;
    private readonly IAppUnitOfWork _appUow;
    private readonly Guid _excerciseRatingId;
    private readonly Guid _userId;
    private readonly Guid _excerciseId;


    public ExcerciseRatingRepoTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;


        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _ctx = new AppDbContext(optionsBuilder.Options);


        var dalMapperConf = new MapperConfiguration(cfg => cfg.AddProfile<DAL.EF.AutoMapperProfile>());


        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _appUow = new AppUOW(_ctx, dalMapperConf.CreateMapper());
        _excerciseRatingId = Guid.NewGuid();
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
            Level = (EExcerciseLevel)1,
            Type = (EExcerciseType)1,
            Description = "Test 1"
        };
        _ctx.Excercise.Add(excercise);
        
        await _ctx.SaveChangesAsync();
        var excerciseRating = new ExcerciseRating()
        {
            Id = _excerciseRatingId,
            Comment = "Test Rating Practice",
            Rating = 3,
            ExcerciseId = _excerciseId,
            UserId = _userId,
            User=appUser,
            Excercise = excercise
        };
        _ctx.ExcerciseRating.Add(excerciseRating);

        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task FirstOrDefaultAsyncTest()
    {
        // ARRANGE
        await SeedData();
        // ACT
        var blogPost = await _appUow.ExcerciseRating.FirstOrDefaultAsync(_excerciseRatingId);
        await _appUow.SaveChangesAsync();
      
        // Assert
        Assert.NotNull(blogPost);
        Assert.That(_excerciseRatingId, Is.EqualTo(blogPost.Id));
    }
    [Fact]
    public async Task GetAllSortedAsyncTest()
    {
        // ARRANGE
        await SeedData();
        // ACT
        var veterynaryPRactices = await _appUow.ExcerciseRating.GetAllAsync();
        await _appUow.SaveChangesAsync();
        // ASSERT
        Assert.NotNull(veterynaryPRactices);
        Assert.That(veterynaryPRactices.Count(), Is.EqualTo(1));
    }
}