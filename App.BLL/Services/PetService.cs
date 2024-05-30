using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.DAL.DTO;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using DAL.Contracts.Repositories;
using Pet = App.BLL.DTO.Pet;

namespace App.BLL.Services;

public class PetService :
    BaseEntityService<App.DAL.DTO.Pet, Pet, IPetRepository>,
    IPetService
{
    private readonly IMapper _mapper;
    public PetService(IAppUnitOfWork uoW, IPetRepository repository, IMapper mapper) : base(uoW,
        repository, new BllDalMapper<App.DAL.DTO.Pet, Pet>(mapper))
    {
        _mapper = mapper;
    }


    public async Task<IEnumerable<Pet>> GetAllPetIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetAllPetIncludingAsync();
        return result.Select(pet => _mapper.Map<DTO.Pet>(pet));
        
    }

    public async Task<IEnumerable<Pet>> GetWithoutCollectionPetIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetWithoutCollectionPetIncludingAsync();
        return result.Select(pet => _mapper.Map<DTO.Pet>(pet));
    }
}