namespace LibraryAuthorization.Infrastructure.Repositories.Interfaces;

public interface IRefreshTokenRedisRepository
{
    public Task SaveAsync(
        long userId,
        Guid tokenId,
        string hash,
        TimeSpan ttl);
    
    public Task<string?> GetHashAsync(int userId, Guid tokenId);

    public Task DeleteAsync(int userId, Guid tokenId);

    public Task DeleteAllAsync(int userId);
}
