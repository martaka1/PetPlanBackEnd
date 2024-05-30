using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using DAL.Contracts.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using DALDTO = App.DAL.DTO;

namespace DAL.App.EF.Repositories;

public class ExcerciseRepository : BaseEntityRepository<Excercise, DALDTO.Excercise, AppDbContext>,
    IExcerciseRepository
{
    private readonly IMapper _mapper;
    public ExcerciseRepository(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<Excercise, DALDTO.Excercise>(mapper))
    {
        _mapper = mapper;
    }

    // implement your custom methods here
    public async Task<IEnumerable<DALDTO.Excercise>> GetAllSortedAsync(Guid userId)
    {
        var query = CreateQuery(userId);
        var res = await query.ToListAsync();
//        query = query.OrderBy(c => c.ContestName);
        
        return res.Select(e => Mapper.Map(e));
    }

    public async Task<IEnumerable<DALDTO.Excercise>> GetAllExcercisesIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var x = await CreateQuery()
            .Include(excercise=>excercise.AppUser)
            .Include(excercise=>excercise.Trainings)
            .ThenInclude(training=>training.HomeTraining)
            .Include(excercise=>excercise.Ratings)
            .ThenInclude(rating=>rating.User)
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<global::App.DAL.DTO.Excercise>>(x)
            : Enumerable.Empty<DALDTO.Excercise>();
    }
}