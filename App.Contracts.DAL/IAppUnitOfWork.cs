
using App.Contracts.DAL.Repositories;
using App.Domain.Identity;
using Base.Contracts.DAL;
using DAL.Contracts.Repositories;

namespace App.Contracts.DAL;

public interface IAppUnitOfWork : IUnitOfWork
{
    // list your repos here

    IPetRepository Pet { get; }
    IAppointmentRepository Appointment { get; }
    IBlogPostRepository BlogPost { get; }
    IBlogTagRepository BlogTag { get; }
    IExcerciseRepository Excercise { get; }
    IHealthRecordRepository HealthRecord { get; }
    IPostTagRepository PostTag { get; }
    IVeterinaryPracticeRepository VeterinaryPractice { get; }
    IEntityRepository<AppUser> Users { get; }
    IBlogPostCommentRepository BlogPostComment { get; }
    IExcerciseRatingRepository ExcerciseRating { get; }
    IVeterinaryPracticeRatingRepository VeterinaryPracticeRating { get; }
    IRefreshTokenRepository RefreshTokens { get; }
}
