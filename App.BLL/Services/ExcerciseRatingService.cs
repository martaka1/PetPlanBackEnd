using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.DTO;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using Base.Contracts.DAL;
using DAL.Contracts.Repositories;

namespace App.BLL.Services;

public class ExcerciseRatingService : BaseEntityService<App.DAL.DTO.ExcerciseRating, App.BLL.DTO.ExcerciseRating, IExcerciseRatingRepository>,
    IExcerciseRatingService
{
    private readonly IMapper _mapper;

    public ExcerciseRatingService(IAppUnitOfWork uoW, IExcerciseRatingRepository repository, IMapper mapper) : base(uoW, repository, new BllDalMapper<ExcerciseRating, DTO.ExcerciseRating>(mapper))
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    public async Task<IEnumerable<DTO.ExcerciseRating>> GetAllExcercisesRatingsCommentsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetAllExcercisesRatingsCommentsIncludingAsync();
        return result.Select(rating => _mapper.Map<DTO.ExcerciseRating>(rating));    }
}