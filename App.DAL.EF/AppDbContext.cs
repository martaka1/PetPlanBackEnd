using App.Domain.Identity;
using Domain.App;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.EF;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid, IdentityUserClaim<Guid>, AppUserRole,
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>

{
    
    public DbSet<Appointment> Appointment {get; set; } = default!;
    public DbSet<BlogPost> BlogPost {get; set; } = default!;
    public DbSet<BlogTag> BlogTag {get; set; } = default!;
    public DbSet<Excercise> Excercise {get; set; } = default!;
    public DbSet<HealthRecord> HealthRecord {get; set; } = default!;
    public DbSet<HomeTraining> HomeTraining {get; set; } = default!;
    public DbSet<Pet> Pet {get; set; } = default!;
    public DbSet<PostTags> PostTags {get; set; } = default!;
    public DbSet<VeterinaryPractice> VeterinaryPractice {get; set; } = default!;
    public DbSet<ExcerciseInTraining> ExcerciseInTrainings { get; set; } = default!;
    public DbSet<Domain.App.PetInHealthRecord> PetInHealthRecord { get; set; } = default!;
    public DbSet<Domain.App.BlogPostComment> BlogPostComment { get; set; } = default!;
    public DbSet<VeterinaryPracticeRating> VeterinaryPracticeRating { get; set; } = default!;
    public DbSet<ExcerciseRating> ExcerciseRating { get; set; } = default!;
    public DbSet<HomeTrainingRating> HomeTrainingRating { get; set; } = default!;
    public DbSet<AppRefreshToken> RefreshTokens { get; set; } = default!;
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entity in ChangeTracker.Entries().Where(e => e.State != EntityState.Deleted))
        {
            foreach (var prop in entity
                         .Properties
                         .Where(x => x.Metadata.ClrType == typeof(DateTime)))
            {
                Console.WriteLine(prop);
                prop.CurrentValue = ((DateTime) prop.CurrentValue).ToUniversalTime();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}