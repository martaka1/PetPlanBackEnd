using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.DAL.DTO;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using DAL.Contracts.Repositories;
using Excercise = App.BLL.DTO.Excercise;

namespace App.BLL.Services;

public class ExcerciseService :
    BaseEntityService<App.DAL.DTO.Excercise, Excercise, IExcerciseRepository>,
    IExcerciseService
{
    private readonly IMapper _mapper;

    public ExcerciseService(IAppUnitOfWork uoW, IExcerciseRepository repository, IMapper mapper) : base(uoW,
        repository, new BllDalMapper<App.DAL.DTO.Excercise, Excercise>(mapper))
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<Excercise>> GetAllExcercisesIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetAllExcercisesIncludingAsync();
        return result.Select(excercise => _mapper.Map<DTO.Excercise>(excercise));
        
    }
}