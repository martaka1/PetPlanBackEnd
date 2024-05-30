using App.BLL.DTO;
using Base.Contracts.DAL;
using DAL.Contracts.Repositories;

namespace App.Contracts.BLL.Services;

public interface IAppointmentService : IEntityRepository<Appointment>, IAppointmentRepositoryCustom<Appointment>
{
 
}