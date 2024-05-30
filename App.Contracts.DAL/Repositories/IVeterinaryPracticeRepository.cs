using Base.Contracts.DAL;
using Domain.App;
using VeterinaryPractice = App.DAL.DTO.VeterinaryPractice;

namespace DAL.Contracts.Repositories;

public interface IVeterinaryPracticeRepository: IEntityRepository<VeterinaryPractice>, IVeterinaryRepositoryCustom<VeterinaryPractice>
{
   
}
public interface IVeterinaryRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllVeterinaryPracticeRatingsIncludingAsync(Guid userId = default, bool noTracking = true);
    
        Task<IEnumerable<TEntity>> GetAllVeterinaryPracticeWithoutCollectionsIncludingAsync(Guid userId = default, bool noTracking = true);

}