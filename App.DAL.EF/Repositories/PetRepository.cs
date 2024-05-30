using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using DAL.Contracts.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using DALDTO = App.DAL.DTO;

namespace DAL.App.EF.Repositories;

public class PetRepository : BaseEntityRepository<Pet, DALDTO.Pet, AppDbContext>,
    IPetRepository
{
    private IMapper _mapper;

    public PetRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<Pet, DALDTO.Pet>(mapper))
    {
        _mapper = mapper;
    }

    // implement your custom methods here
    public async Task<IEnumerable<DALDTO.Pet>> GetAllSortedAsync(Guid userId)
    {
        var query = CreateQuery(userId);
        var res = await query.ToListAsync();
//        query = query.OrderBy(c => c.ContestName);
        
        return res.Select(e => Mapper.Map(e))!;
    }

    public async Task<IEnumerable<DALDTO.Pet>> GetAllPetIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var x = await CreateQuery()
            .Include(p=>p.AppUser)
            .Include(p=>p.HealthRecords)!
            .ThenInclude(hr=>hr.AppUser)
            .Include(p=>p.Appointments)!
            .ThenInclude(a=>a.AppUser)
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<DALDTO.Pet>>(x)
            : Enumerable.Empty<DALDTO.Pet>();
    }
    public async Task<IEnumerable<DALDTO.Pet>> GetWithoutCollectionPetIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var x = await CreateQuery()
            .Include(p=>p.AppUser)
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<DALDTO.Pet>>(x)
            : Enumerable.Empty<DALDTO.Pet>();
    }
}
