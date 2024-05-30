using App.DAL.EF;
using AutoMapper;
using Base.DAL.EF;
using DAL.Contracts.Repositories;
using Domain.App;
using Microsoft.EntityFrameworkCore;
using DALDTO = App.DAL.DTO;

namespace DAL.App.EF.Repositories;

public class VeterinaryPracticeRatingRepository:BaseEntityRepository<VeterinaryPracticeRating,DALDTO.VeterinaryPracticeRating,AppDbContext>,
    IVeterinaryPracticeRatingRepository
{
    private readonly IMapper _mapper;
    public VeterinaryPracticeRatingRepository(AppDbContext dbContext, IMapper mapper) : 
        base(dbContext, new DalDomainMapper<VeterinaryPracticeRating, DALDTO.VeterinaryPracticeRating>(mapper))
    {
        _mapper = mapper;
    }


    public async Task<IEnumerable<DALDTO.VeterinaryPracticeRating>> GetAllVeterinaryPracticeRatingsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var x = await CreateQuery()
            .Include(rating=>rating.User)
            .ToListAsync();
        var result = x.Select(de => Mapper.Map(de));
        return result != null
            ? _mapper.Map<IEnumerable<DALDTO.VeterinaryPracticeRating>>(x)
            : Enumerable.Empty<DALDTO.VeterinaryPracticeRating>();
    }
}