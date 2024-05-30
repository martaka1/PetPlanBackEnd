using App.DAL.DTO;
using App.Domain.Identity;
using AutoMapper;
using Domain.App;
using Domain.App.HelperEnums;
using Appointment = Domain.App.Appointment;
using BlogPost = Domain.App.BlogPost;
using BlogPostComment = Domain.App.BlogPostComment;
using Excercise = Domain.App.Excercise;
using ExcerciseInTraining = Domain.App.ExcerciseInTraining;
using ExcerciseRating = Domain.App.ExcerciseRating;
using HealthRecord = Domain.App.HealthRecord;
using HomeTraining = Domain.App.HomeTraining;
using HomeTrainingRating = Domain.App.HomeTrainingRating;
using Pet = Domain.App.Pet;
using PetInHealthRecord = Domain.App.PetInHealthRecord;
using PostTags = Domain.App.PostTags;
using VeterinaryPractice = Domain.App.VeterinaryPractice;
using VeterinaryPracticeRating = Domain.App.VeterinaryPracticeRating;


namespace App.DAL.EF;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<App.Domain.Identity.AppRefreshToken, App.DAL.DTO.Identity.AppRefreshToken>().ReverseMap();
        CreateMap<App.Domain.Identity.AppUser, App.DAL.DTO.Identity.AppUser>().ReverseMap();
        
        CreateMap<Pet, App.DAL.DTO.Pet>().ReverseMap();
        CreateMap<Pet, App.DAL.DTO.PetWithoutCollection>().ReverseMap();
        
        CreateMap<Appointment,App.DAL.DTO.Appointment>().ReverseMap();
        
        CreateMap<BlogPost,App.DAL.DTO.BlogPost>().ReverseMap();
        CreateMap<BlogPost,App.DAL.DTO.BlogPostWithoutCollection>().ReverseMap();
        CreateMap<BlogPostComment, App.DAL.DTO.BlogPostComment>().ReverseMap();
        
        CreateMap<BlogTag,App.DAL.DTO.BlogPostTag>().ReverseMap();
        CreateMap<BlogTag,App.DAL.DTO.BlogPostTagWithoutCollection>().ReverseMap();
        
        CreateMap<Excercise,App.DAL.DTO.Excercise>().ReverseMap();
        CreateMap<Excercise,App.DAL.DTO.ExcerciseWithoutCollection>().ReverseMap();
        CreateMap<ExcerciseRating,App.DAL.DTO.ExcerciseRating>().ReverseMap();
        
        
        CreateMap<HealthRecord,App.DAL.DTO.HealthRecord>().ReverseMap();
       
        
        CreateMap<PostTags,App.DAL.DTO.PostTags>().ReverseMap();
        
        CreateMap<VeterinaryPractice,App.DAL.DTO.VeterinaryPractice>().ReverseMap();
        CreateMap<VeterinaryPractice,App.DAL.DTO.VeterinaryPracticeWithoutCollection>().ReverseMap();
        CreateMap<VeterinaryPracticeRating,App.DAL.DTO.VeterinaryPracticeRating>().ReverseMap();

        CreateMap<EAppointmentType, DTO.HelperEnums.EAppointmentType>().ReverseMap();
        CreateMap<EExcerciseLevel, DTO.HelperEnums.EExcerciseLevel>().ReverseMap();
        CreateMap<EExcerciseType, DTO.HelperEnums.EExcerciseType>().ReverseMap();
    }
}