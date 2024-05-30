using App.Contracts.BLL.Services;
using Base.Contracts.BLL;
using DAL.Contracts.Repositories;

namespace App.Contracts.BLL;

public interface IAppBLL : IBLL
{
    IAppointmentService? Appointment { get; }
    IBlogPostService BlogPost { get; }
    IBlogPostCommentService BlogPostComment { get; }
    IBlogTagService BlogTag { get; }
    IExcerciseService Excercise { get; }
    IExcerciseRatingService ExcerciseRating { get; }
    IHealthRecordService HealthRecord { get; }
    IPetService Pet { get; }
    IPostTagService PostTag { get; } 
    IVeterinaryPracticeService VeterinaryPractice { get; }
    IVeterinaryPracticeRatingService VeterinaryPracticeRating { get; }
    IRefreshTokenService  AppRefreshTokens { get; }

}