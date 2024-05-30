using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.DAL.DTO;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using DAL.Contracts.Repositories;
using VeterinaryPractice = App.BLL.DTO.VeterinaryPractice;

namespace App.BLL.Services;

public class VeterinaryPracticeService :
    BaseEntityService<App.DAL.DTO.VeterinaryPractice, VeterinaryPractice, IVeterinaryPracticeRepository>,
    IVeterinaryPracticeService
{
    private readonly IMapper _mapper;

    public VeterinaryPracticeService(IAppUnitOfWork uoW, IVeterinaryPracticeRepository repository, IMapper mapper) : base(uoW,
        repository, new BllDalMapper<App.DAL.DTO.VeterinaryPractice, VeterinaryPractice>(mapper))
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<VeterinaryPractice>> GetAllVeterinaryPracticeRatingsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetAllVeterinaryPracticeRatingsIncludingAsync();
        return result.Select(rating => _mapper.Map<DTO.VeterinaryPractice>(rating));    }
    
    
        
    public async Task<IEnumerable<VeterinaryPractice>> GetAllVeterinaryPracticeWithoutCollectionsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetAllVeterinaryPracticeWithoutCollectionsIncludingAsync();
        return result.Select(rating => _mapper.Map<DTO.VeterinaryPractice>(rating));    }
}