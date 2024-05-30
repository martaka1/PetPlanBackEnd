using Base.Contracts.DAL;

namespace DAL.Contracts.Repositories;

public interface IVeterinaryPracticeRatingRepository : IEntityRepository<App.DAL.DTO.VeterinaryPracticeRating>, IVeterinaryPracticeRatingRepositoryCustom<App.DAL.DTO.VeterinaryPracticeRating>
{
    
}


public interface IVeterinaryPracticeRatingRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllVeterinaryPracticeRatingsIncludingAsync(Guid userId = default, bool noTracking = true);
}