using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using DAL.Contracts.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using DALDTO = App.DAL.DTO;

namespace DAL.App.EF.Repositories;

public class AppointmentRepository : BaseEntityRepository<Appointment, DALDTO.Appointment, AppDbContext>,
    IAppointmentRepository
{
    private readonly IMapper _mapper;
    public AppointmentRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<Appointment, DALDTO.Appointment>(mapper))
    {
        _mapper = mapper;
    }

    // implement your custom methods here
    public async Task<IEnumerable<DALDTO.Appointment>> GetAllSortedAsync(Guid userId)
    {
        var query = CreateQuery(userId);
        var res = await query.ToListAsync();
//        query = query.OrderBy(c => c.ContestName);
        
        return res.Select(e => Mapper.Map(e));
    }

    public async Task<IEnumerable<global::App.DAL.DTO.Appointment>> GetAllAppointmentsIncludedAsync()
    {
        var appointments = await CreateQuery()
            .Include(appointment => appointment.AppUser)
            .Include(appointment => appointment.Pet)
            .ToListAsync();

        var result = appointments.Select(de => Mapper.Map(de));

        return result != null
            ? _mapper.Map<IEnumerable<global::App.DAL.DTO.Appointment>>(appointments)
            : Enumerable.Empty<global::App.DAL.DTO.Appointment>();

    }
    
}