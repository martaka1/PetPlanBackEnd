using BLL.Services;
using Contracts.BLL;
using Contracts.BLL.Services;
using Contracts.DAL;
using DAL.EF;
using AutoMapper;
using BLL;
using DAL.App.EF;
using DAL.Contracts;


namespace BLL;

public class AppBLL: BaseBLL<AppDbContext>, IAppBLL
{
    private readonly IMapper _mapper;
    private readonly IAppUnitOfWork _uow;
    
    public AppBLL(IAppUnitOfWork uoW, IMapper mapper) : base(uoW)
    {
        _mapper = mapper;
        _uow = uoW;
    }

    private IPetService? _pet;
    public IPetService Contests => _pet ?? new PetService(_uow, _uow.Pet, _mapper);
}
