using App.BLL.DTO;
using Base.Contracts.DAL;
using DAL.Contracts.Repositories;


namespace App.Contracts.BLL.Services;

public interface IVeterinaryPracticeRatingService : IEntityRepository<VeterinaryPracticeRating>, IVeterinaryPracticeRatingRepositoryCustom<VeterinaryPracticeRating>
{
  
}