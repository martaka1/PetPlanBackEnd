using App.DAL.DTO;
using App.BLL.DTO;
using App.BLL.DTO.Identity;
using AutoMapper;
using DALDTO = App.DAL.DTO;
using BLLDTO =App.BLL.DTO;


namespace App.BLL;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<App.DAL.DTO.Identity.AppRefreshToken, App.BLL.DTO.AppRefreshToken>().ReverseMap();
        CreateMap<DALDTO.Identity.AppUser, AppUser>().ReverseMap();

        CreateMap<DALDTO.Appointment, BLLDTO.Appointment>().ReverseMap();
        
        CreateMap<DALDTO.BlogPost, BLLDTO.BlogPost>().ReverseMap();
        CreateMap<DALDTO.BlogPostWithoutCollection, BLLDTO.BlogPost>().ReverseMap();
        CreateMap<DALDTO.BlogPostComment, BLLDTO.BlogPostComment>().ReverseMap();
        
        CreateMap<DALDTO.BlogPostTag, BLLDTO.BlogTag>().ReverseMap();
        CreateMap<DALDTO.BlogPostTagWithoutCollection, BLLDTO.BlogTag>().ReverseMap();
        
        CreateMap<DALDTO.Excercise, BLLDTO.Excercise>().ReverseMap();
        CreateMap<DALDTO.ExcerciseWithoutCollection, BLLDTO.Excercise>().ReverseMap();
        CreateMap<DALDTO.ExcerciseRating, BLLDTO.ExcerciseRating>().ReverseMap();

        CreateMap<DALDTO.HealthRecord, BLLDTO.HealthRecord>().ReverseMap();
        
        
        CreateMap<DALDTO.Pet, BLLDTO.Pet>().ReverseMap();
        CreateMap<DALDTO.PetWithoutCollection, BLLDTO.Pet>().ReverseMap();
        
        CreateMap<DALDTO.PostTags, BLLDTO.PostTags>().ReverseMap();
        
        CreateMap<DALDTO.VeterinaryPractice, BLLDTO.VeterinaryPractice>().ReverseMap();
        CreateMap<DALDTO.VeterinaryPracticeWithoutCollection, BLLDTO.VeterinaryPractice>().ReverseMap();
        CreateMap<DALDTO.VeterinaryPracticeRating, BLLDTO.VeterinaryPracticeRating>().ReverseMap();
        
        CreateMap<App.DAL.DTO.HelperEnums.EExcerciseType, App.BLL.DTO.HelperEnums.EExcerciseType>();
        CreateMap<App.DAL.DTO.HelperEnums.EExcerciseLevel, App.BLL.DTO.HelperEnums.EExcerciseType>();
        CreateMap<App.DAL.DTO.HelperEnums.EAppointmentType, App.BLL.DTO.HelperEnums.EAppointmentType>();
    }
}