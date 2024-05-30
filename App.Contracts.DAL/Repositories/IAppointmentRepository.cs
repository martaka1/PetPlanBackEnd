using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;


namespace DAL.Contracts.Repositories;


public interface IAppointmentRepository : IEntityRepository<DALDTO.Appointment>, IAppointmentRepositoryCustom<DALDTO.Appointment>
{
    // define your custom methods here
}
public interface IAppointmentRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllAppointmentsIncludedAsync();
}