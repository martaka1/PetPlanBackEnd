using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using DAL.Contracts.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using DALDTO = App.DAL.DTO;

namespace DAL.App.EF.Repositories;

public class ExcerciseRatingRepository:BaseEntityRepository<ExcerciseRating,DALDTO.ExcerciseRating,AppDbContext>,
    IExcerciseRatingRepository
{
    private readonly IMapper _mapper;
    public ExcerciseRatingRepository(AppDbContext dbContext, IMapper mapper) : 
        base(dbContext, new DalDomainMapper<ExcerciseRating, DALDTO.ExcerciseRating>(mapper))
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<DALDTO.ExcerciseRating>> GetAllExcercisesRatingsCommentsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var x = await CreateQuery()
            .Include(rating=>rating.User)
            .Include(rating=>rating.Excercise)
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<DALDTO.ExcerciseRating>>(x)
            : Enumerable.Empty<DALDTO.ExcerciseRating>();
    }
}