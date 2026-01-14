using LibraryAuthorization.Infrastructure.Repositories.Interfaces;

using StackExchange.Redis;

namespace LibraryAuthorization.Infrastructure.Repositories;

public class RefreshTokenRedisRepository : IRefreshTokenRedisRepository
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _redis;

    public RefreshTokenRedisRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }

    public async Task SaveAsync(
        long userId,
        Guid tokenId,
        string hash,
        TimeSpan ttl)
    {
        await _db.StringSetAsync(
            $"refresh:{userId}:{tokenId}",
            hash,
            ttl);
    }

    public async Task<string?> GetHashAsync(int userId, Guid tokenId)
    {
        var value = await _db.StringGetAsync(
            $"refresh:{userId}:{tokenId}");

        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task DeleteAsync(int userId, Guid tokenId)
    {
        await _db.KeyDeleteAsync($"refresh:{userId}:{tokenId}");
    }

    public async Task DeleteAllAsync(int userId)
    {
        var server = _redis.GetServer(
            _redis.GetEndPoints().First());

        var keys = server.Keys(
            pattern: $"refresh:{userId}:*");

        foreach (var key in keys)
            await _db.KeyDeleteAsync(key);
    }
}
