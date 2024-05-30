using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;


namespace DAL.Contracts.Repositories;


public interface IPetRepository : IEntityRepository<DALDTO.Pet>, IPetRepositoryCustom<DALDTO.Pet>
{
    // define your custom methods here

    
}
public interface IPetRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllPetIncludingAsync(Guid userId = default, bool noTracking = true);
    Task<IEnumerable<TEntity>> GetWithoutCollectionPetIncludingAsync(Guid userId = default, bool noTracking = true);
}