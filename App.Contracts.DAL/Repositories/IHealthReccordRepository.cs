using Base.Contracts.DAL;
using Domain.App;
using HealthRecord = App.DAL.DTO.HealthRecord;

namespace DAL.Contracts.Repositories;

public interface IHealthRecordRepository : IEntityRepository<HealthRecord>, IHealthRecordRepositoryCustom<HealthRecord>
{
    // define your custom methods here

}
public interface IHealthRecordRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllHealthRecordIncludingAsync(Guid userId = default, bool noTracking = true);
    Task<IEnumerable<TEntity>> GetAllHealthRecordwithoutcollectionIncludingAsync(Guid userId = default, bool noTracking = true);
    Task<IEnumerable<TEntity>> GetAllHealthRecordWithPetId(Guid userId = default, bool noTracking = true);

}