using App.BLL.DTO.HelperEnums;
using App.BLL.DTO.Identity;
using App.Domain.Identity;
using DAL.App.EF;
using Domain.App;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AppUser = App.Domain.Identity.AppUser;
using EExcerciseLevel = Domain.App.HelperEnums.EExcerciseLevel;
using EExcerciseType = Domain.App.HelperEnums.EExcerciseType;

namespace App.Test;

// ReSharper disable once ClassNeverInstantiated.Global
public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup: class
{
    private static bool dbInitialized = false;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    { 
        builder.ConfigureServices(async services =>
        {
            // Change the DI container registrations
            
            // Find DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<AppDbContext>));

            // If found, remove it
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add new DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Create db and seed data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();
            var logger = scopedServices
                .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

            db.Database.EnsureCreated();
            
            try
            {
                if (dbInitialized == false)
                {
                    dbInitialized = true;

                    var adminUserId = Guid.Parse("75b1a4c7-89d0-4f31-9f4f-19f4c6c85977");
                    var regularUserId = Guid.NewGuid();

                    var adminUser = new AppUser
                    {
                        Id = adminUserId,
                        FirstName = "Admin",
                        LastName = "Eesti",
                        Email = "admin@eesti.ee"
                    };

                    var regularUser = new AppUser
                    {
                        Id = regularUserId,
                        FirstName = "Mimmu",
                        LastName = "Mammu",
                        Email = "mimmu@mammu.ee"
                    };

                    if (!db.Users.Any())
                    {
                        db.Users.AddRange(adminUser, regularUser);
                        await db.SaveChangesAsync();
                    }

                    if (!db.BlogPost.Any())
                    {
                        var blogPostId = Guid.Parse("f2786165-3ffc-45bb-9547-c955135d16ab");
                        var newBlogPost = new BlogPost
                        {
                            Id = blogPostId,
                            Title = "Test",
                            Summary = "This is a testing blogPost",
                            Content = "Test test test test test test test test.",
                            AppUser = adminUser,
                            AppUserId = adminUserId
                        };
                    
                        db.BlogPost.Add(newBlogPost);
                        await db.SaveChangesAsync();

                        var commentId = Guid.NewGuid();
                        var comment = new BlogPostComment
                        {
                            Id = commentId,
                            Comment = "Test Rating Practice",
                            BlogPostId = blogPostId,
                            UserId = regularUserId,
                            BlogPost = newBlogPost,
                            User = regularUser,
                        };
                        
                        db.BlogPostComment.Add(comment);
                        await db.SaveChangesAsync();
                        if (!db.BlogTag.Any())
                        {
                            var tag1 = new BlogTag { Name = "Dog" };
                            var tag2 = new BlogTag { Name = "Cat" };
                            var tag3 = new BlogTag { Name = "Walking" };

                            db.BlogTag.AddRange(tag1, tag2, tag3);
                        }
                        await db.SaveChangesAsync();
                    }
                    
                    if (!db.ExcerciseRating.Any())
                    {
                        var excerciseId = Guid.NewGuid();
                        var excercise = new Excercise
                        {
                            Id = excerciseId,
                            Name = "Sample Exercise",
                            Description = "This is a sample exercise", 
                            Type= (EExcerciseType)BLL.DTO.HelperEnums.EExcerciseType.Inside,
                            Level = (EExcerciseLevel)BLL.DTO.HelperEnums.EExcerciseLevel.Beginner
                        };

                        db.Excercise.Add(excercise);
                        await db.SaveChangesAsync();

                        var excerciseRating = new ExcerciseRating
                        {
                            Id = new Guid("faed5d2d-6930-48d0-96cc-123f4f6d123e"),
                            Comment = "Test Rating Practice",
                            Rating = 3,
                            ExcerciseId = excerciseId,
                            UserId = regularUserId,
                            User = regularUser,
                            Excercise = excercise
                        };

                        db.ExcerciseRating.Add(excerciseRating);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the " +
                                    "database with test data. Error: {Message}", ex.Message);
            }
        });
    }
}
