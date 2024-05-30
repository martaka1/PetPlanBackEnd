using AutoMapper;

namespace WebApp.Helpers;

public class AutoMapperProfile: Profile
{
    public AutoMapperProfile()
    {
        CreateMap<App.BLL.DTO.AppRefreshToken, App.DTO.v1_0.Identity.AppRefreshToken>().ReverseMap();
        CreateMap<App.BLL.DTO.Identity.AppUser, App.DTO.v1_0.Identity.AppUser>().ReverseMap();

        CreateMap<App.BLL.DTO.Appointment, App.DTO.v1_0.Appointment>().ReverseMap();

        CreateMap<App.BLL.DTO.BlogPost, App.DTO.v1_0.BlogPost>().ReverseMap();
        CreateMap<App.BLL.DTO.BlogPost, App.DTO.v1_0.BlogPostWithoutCollection>().ReverseMap();
        CreateMap<App.BLL.DTO.BlogPostComment, App.DTO.v1_0.BlogPostComment>().ReverseMap();
        CreateMap<App.BLL.DTO.BlogTag, App.DTO.v1_0.BlogTag>().ReverseMap();
        CreateMap<App.BLL.DTO.BlogTag, App.DTO.v1_0.BlogTagWithoutCollection>().ReverseMap();

        CreateMap<App.BLL.DTO.Excercise, App.DTO.v1_0.Excercise>().ReverseMap();
        CreateMap<App.BLL.DTO.Excercise, App.DTO.v1_0.ExcerciseWithoutCollection>().ReverseMap();
        CreateMap<App.BLL.DTO.ExcerciseRating, App.DTO.v1_0.ExcerciseRating>().ReverseMap();
        CreateMap<App.BLL.DTO.ExcerciseInTraining, App.DTO.v1_0.ExcerciseInTraining>().ReverseMap();
        
        CreateMap<App.BLL.DTO.HealthRecord, App.DTO.v1_0.HealthRecord>().ReverseMap();

        CreateMap<App.BLL.DTO.HomeTraining, App.DTO.v1_0.HomeTraining>().ReverseMap();
        CreateMap<App.BLL.DTO.HomeTraining, App.DTO.v1_0.HomeTrainingWithoutCollection>().ReverseMap();
        CreateMap<App.BLL.DTO.HomeTrainingRating, App.DTO.v1_0.HomeTrainingRating>().ReverseMap();
        
        CreateMap<App.BLL.DTO.Pet, App.DTO.v1_0.Pet>().ReverseMap();
        CreateMap<App.BLL.DTO.Pet, App.DTO.v1_0.PetWithoutCollection>().ReverseMap();
        
        CreateMap<App.BLL.DTO.PostTags, App.DTO.v1_0.PostTags>().ReverseMap();
        
        CreateMap<App.BLL.DTO.VeterinaryPractice, App.DTO.v1_0.VeterinaryPractice>().ReverseMap();
        CreateMap<App.BLL.DTO.VeterinaryPractice, App.DTO.v1_0.VeterinaryPracticeWithoutCollection>().ReverseMap();
        CreateMap<App.BLL.DTO.VeterinaryPracticeRating, App.DTO.v1_0.VeterinaryPracticeRating>().ReverseMap();

        CreateMap<App.BLL.DTO.HelperEnums.EAppointmentType, App.DTO.v1_0.HelperEnums.EAppointmentType>().ReverseMap();
        CreateMap<App.BLL.DTO.HelperEnums.EExcerciseType, App.DTO.v1_0.HelperEnums.EExcerciseType>().ReverseMap();
        CreateMap<App.BLL.DTO.HelperEnums.EExcerciseLevel, App.DTO.v1_0.HelperEnums.EExcerciseLevel>().ReverseMap();
    }
}