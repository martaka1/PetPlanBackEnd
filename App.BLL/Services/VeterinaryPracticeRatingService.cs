
using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.DTO;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using Base.Contracts.DAL;
using DAL.Contracts.Repositories;
using VeterinaryPracticeRating = App.BLL.DTO.VeterinaryPracticeRating;

namespace App.BLL.Services;

public class VeterinaryPracticeRatingService : BaseEntityService<App.DAL.DTO.VeterinaryPracticeRating, App.BLL.DTO.VeterinaryPracticeRating, IVeterinaryPracticeRatingRepository>,
    IVeterinaryPracticeRatingService
{
    private readonly IMapper _mapper;

    public VeterinaryPracticeRatingService(IAppUnitOfWork uoW, IVeterinaryPracticeRatingRepository repository, IMapper mapper) : base(uoW, repository, new BllDalMapper<App.DAL.DTO.VeterinaryPracticeRating, DTO.VeterinaryPracticeRating>(mapper))
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    

    public async Task<IEnumerable<DTO.VeterinaryPracticeRating>> GetAllVeterinaryPracticeRatingsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetAllVeterinaryPracticeRatingsIncludingAsync();
        return result.Select(rating => _mapper.Map<DTO.VeterinaryPracticeRating>(rating));
        
    }
}
