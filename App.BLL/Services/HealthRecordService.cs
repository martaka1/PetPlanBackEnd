using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.DAL.DTO;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using DAL.Contracts.Repositories;
using HealthRecord = App.BLL.DTO.HealthRecord;

namespace App.BLL.Services;

public class HealthRecordService :
    BaseEntityService<App.DAL.DTO.HealthRecord, HealthRecord, IHealthRecordRepository>,
    IHealthRecordService
{
    private readonly IMapper _mapper;

    public HealthRecordService(IAppUnitOfWork uoW, IHealthRecordRepository repository, IMapper mapper) : base(uoW,
        repository, new BllDalMapper<App.DAL.DTO.HealthRecord, HealthRecord>(mapper))
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<HealthRecord>> GetAllHealthRecordIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetAllHealthRecordIncludingAsync();
        return result.Select(healthRecord => _mapper.Map<DTO.HealthRecord>(healthRecord));    }
    
    public async Task<IEnumerable<HealthRecord>> GetAllHealthRecordwithoutcollectionIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetAllHealthRecordwithoutcollectionIncludingAsync();
        return result.Select(healthRecord => _mapper.Map<DTO.HealthRecord>(healthRecord));    }
    
    public async Task<IEnumerable<HealthRecord>> GetAllHealthRecordWithPetId(Guid userId = default, bool noTracking = true)
    {
        var result = await Repository.GetAllHealthRecordWithPetId();
        return result.Select(healthRecord => _mapper.Map<DTO.HealthRecord>(healthRecord));    }
}