using Base.Contracts.DAL;
using Domain.App;
using Excercise = App.DAL.DTO.Excercise;

namespace DAL.Contracts.Repositories;

public interface IExcerciseRepository : IEntityRepository<Excercise>, IExcerciseRepositoryCustom<Excercise>
{
    // define your custom methods here
}
public interface IExcerciseRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllExcercisesIncludingAsync(Guid userId = default, bool noTracking = true);

}