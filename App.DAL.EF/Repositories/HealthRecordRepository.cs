using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using DAL.Contracts.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using DALDTO = App.DAL.DTO;

namespace DAL.App.EF.Repositories;

public class HealthRecordRepository : BaseEntityRepository<HealthRecord, DALDTO.HealthRecord, AppDbContext>,
    IHealthRecordRepository
{
    private readonly IMapper _mapper;
    public HealthRecordRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<HealthRecord, DALDTO.HealthRecord>(mapper))
    {
        _mapper = mapper;
    }

    // implement your custom methods here
    public async Task<IEnumerable<DALDTO.HealthRecord>> GetAllSortedAsync(Guid userId)
    {
        var query = CreateQuery(userId);
        var res = await query.ToListAsync();
//        query = query.OrderBy(c => c.ContestName);
        
        return res.Select(e => Mapper.Map(e));
    }

    public async Task<IEnumerable<DALDTO.HealthRecord>> GetAllHealthRecordIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var x = await CreateQuery()
            .Include(hr=>hr.AppUser)
            .Include(hr=>hr.Pet)
            .Include(hr=>hr.VeterinaryPractice)
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<DALDTO.HealthRecord>>(x)
            : Enumerable.Empty<DALDTO.HealthRecord>();
    }
    
    public async Task<IEnumerable<DALDTO.HealthRecord>> GetAllHealthRecordwithoutcollectionIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var x = await CreateQuery()
            .Include(hr=>hr.AppUser)
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<DALDTO.HealthRecord>>(x)
            : Enumerable.Empty<DALDTO.HealthRecord>();
    }
    public async Task<IEnumerable<DALDTO.HealthRecord>> GetAllHealthRecordWithPetId(Guid userId = default, bool noTracking = true)
    {
        var x = await CreateQuery()
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<DALDTO.HealthRecord>>(x)
            : Enumerable.Empty<DALDTO.HealthRecord>();
    }
}