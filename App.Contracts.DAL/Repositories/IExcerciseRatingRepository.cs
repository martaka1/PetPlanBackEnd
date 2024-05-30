using Base.Contracts.DAL;

namespace DAL.Contracts.Repositories;

public interface IExcerciseRatingRepository : IEntityRepository<App.DAL.DTO.ExcerciseRating>, IExcerciseRatingRepositoryCustom<App.DAL.DTO.ExcerciseRating>
{
    
}


public interface IExcerciseRatingRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllExcercisesRatingsCommentsIncludingAsync(Guid userId = default, bool noTracking = true);
}