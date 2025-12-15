using LibraryAuthorization.Domain.Entities;
using LibraryAuthorization.Infrastructure.Data;
using LibraryAuthorization.Infrastructure.Repositories.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace LibraryAuthorization.Infrastructure.Repositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AuthDbContext context) : base(context) {}

    public async Task<RefreshToken?> FindDetailedEntityByToken(string token)
    {
        return await _dbSet
            .Include(t => t.User)
            .SingleOrDefaultAsync(t => t.Token == token && !t.IsRevoked);
    }
}
