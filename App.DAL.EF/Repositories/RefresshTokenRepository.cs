using App.Contracts.DAL.Repositories;
using App.DAL.EF;
using App.Domain.Identity;
using AutoMapper;
using Base.DAL.EF;

namespace DAL.App.EF.Repositories;

public class IdentityRepo : BaseEntityRepository<AppRefreshToken, global::App.DAL.DTO.Identity.AppRefreshToken, AppDbContext>, 
    IRefreshTokenRepository 
    
{
    public IdentityRepo(AppDbContext dbContext, IMapper mapper) :
        base(dbContext, new DalDomainMapper<global::App.Domain.Identity.AppRefreshToken, global::App.DAL.DTO.Identity.AppRefreshToken>(mapper))
    {
    }

    public async Task<bool> isValid(string refreshToken)
    {
        var query = GetAll();
        var found = query.FirstOrDefault(token => token.RefreshToken == refreshToken);
        if (found == null) return false;
        return await Task.Run(() => found.ExpinationDT > DateTime.Now);
    }
}