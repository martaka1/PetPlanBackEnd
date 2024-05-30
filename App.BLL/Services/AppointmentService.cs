using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.DAL.DTO;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using DAL.Contracts.Repositories;
using Appointment = App.BLL.DTO.Appointment;

namespace App.BLL.Services;

public class AppointmentService :
    BaseEntityService<App.DAL.DTO.Appointment, Appointment, IAppointmentRepository>,
    IAppointmentService
{
    private readonly IMapper _mapper;
    public AppointmentService(IAppUnitOfWork uoW, IAppointmentRepository repository, IMapper mapper) : base(uoW,
        repository, new BllDalMapper<App.DAL.DTO.Appointment, Appointment>(mapper))
    {
        _mapper = mapper;
    }
    

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsIncludedAsync()
    {
        var result = await Repository.GetAllAppointmentsIncludedAsync();
        return result.Select(appointment => _mapper.Map<App.BLL.DTO.Appointment>(appointment));
    }
}