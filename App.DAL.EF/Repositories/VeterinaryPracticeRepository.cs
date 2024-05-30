using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using DAL.Contracts.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using DALDTO = App.DAL.DTO;

namespace DAL.App.EF.Repositories;

public class VeterinaryPracticeRepository : BaseEntityRepository<VeterinaryPractice, DALDTO.VeterinaryPractice, AppDbContext>,
    IVeterinaryPracticeRepository
{
    private readonly IMapper _mapper;
    public VeterinaryPracticeRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<VeterinaryPractice, DALDTO.VeterinaryPractice>(mapper))
    {
        _mapper = mapper;
    }

    // implement your custom methods here
    public async Task<IEnumerable<DALDTO.VeterinaryPractice>> GetAllSortedAsync(Guid userId)
    {
        var query = CreateQuery(userId);
        var res = await query.ToListAsync();
//        query = query.OrderBy(c => c.ContestName);
        
        return res.Select(e => Mapper.Map(e));
    }

    public async Task<IEnumerable<DALDTO.VeterinaryPractice>> GetAllVeterinaryPracticeRatingsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var x = await CreateQuery()
            .Include(vp=>vp.HealthRecords)
            .ThenInclude(hr=>hr.AppUser)
            .Include(vp=>vp.VeterinaryPracticeRatings)
            .ThenInclude(vpr=>vpr.User)
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<global::App.DAL.DTO.VeterinaryPractice>>(x)
            : Enumerable.Empty<DALDTO.VeterinaryPractice>();
    }
    public async Task<IEnumerable<DALDTO.VeterinaryPractice>> GetAllVeterinaryPracticeWithoutCollectionsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var x = await CreateQuery()
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<global::App.DAL.DTO.VeterinaryPractice>>(x)
            : Enumerable.Empty<DALDTO.VeterinaryPractice>();
    }
}