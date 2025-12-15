using LibraryAuthorization.Domain.Entities;

namespace LibraryAuthorization.Infrastructure.Repositories.Interfaces;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    public Task<RefreshToken?> FindDetailedEntityByToken(string token);
}
