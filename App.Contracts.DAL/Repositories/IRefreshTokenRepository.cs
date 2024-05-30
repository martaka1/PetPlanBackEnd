using App.Domain.Identity;
using Base.Contracts.DAL;

namespace App.Contracts.DAL.Repositories;

public interface IRefreshTokenRepository : IEntityRepository<App.DAL.DTO.Identity.AppRefreshToken>, IRefreshTokenRepositoryCustom<Guid>
{
    
}

public interface IRefreshTokenRepositoryCustom<TEntity>
{
    Task<bool> isValid(string refreshToken);
}