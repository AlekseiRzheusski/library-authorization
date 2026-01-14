using LibraryAuthorization.Infrastructure.Repositories;
using LibraryAuthorization.Integration.Tests.Fixtures;

namespace LibraryAuthorization.Integration.Tests.Infrastructure;

public class RefreshTokenRedisRepositoryTests
    : IClassFixture<RedisFixture>
{
    private readonly RedisFixture _fixture;
    private readonly RefreshTokenRedisRepository _repository;

    public RefreshTokenRedisRepositoryTests(RedisFixture fixture)
    {
        _fixture = fixture;
        _repository = new RefreshTokenRedisRepository(
            fixture.Redis);
    }

    [Fact]
    public async Task SaveAsync_ShouldAddKey()
    {
        var userId = 1;
        var tokenId = Guid.NewGuid();
        var hash = "hash123";

        await _repository.SaveAsync(
            userId,
            tokenId,
            hash,
            TimeSpan.FromSeconds(5));

        var result = await _repository.GetHashAsync(
            userId,
            tokenId);

        Assert.Equal(hash, result);
    }

    [Fact]
    public async Task SaveAsync_ShouldSetTtl()
    {
        var userId = 1;
        var tokenId = Guid.NewGuid();
        var expectedTtl = TimeSpan.FromSeconds(10);

        await _repository.SaveAsync(
            userId,
            tokenId,
            "hash456",
            expectedTtl);

        var ttl = await _fixture.Redis.GetDatabase().KeyTimeToLiveAsync(
            $"refresh:{userId}:{tokenId}");

        Assert.NotNull(ttl);
        Assert.InRange(
            ttl!.Value,
            expectedTtl - TimeSpan.FromSeconds(5),
            expectedTtl);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveKey()
    {
        var userId = 1;
        var tokenId = Guid.NewGuid();

        await _repository.SaveAsync(
            userId,
            tokenId,
            "hash",
            TimeSpan.FromMinutes(1));

        await _repository.DeleteAsync(
            userId,
            tokenId);

        var result = await _repository.GetHashAsync(
            userId,
            tokenId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAllAsync_ShouldRemoveAllUserTokens()
    {
        var userId = 2;

        await _repository.SaveAsync(
            userId,
            Guid.NewGuid(),
            "hash1",
            TimeSpan.FromMinutes(1));

        await _repository.SaveAsync(
            userId,
            Guid.NewGuid(),
            "hash2",
            TimeSpan.FromMinutes(1));

        await _repository.DeleteAllAsync(userId);

        var server = _fixture.Redis.GetServer(
            _fixture.Redis.GetEndPoints().First());

        var keys = server.Keys(
            pattern: $"refresh:{userId}:*");

        Assert.Empty(keys);
    }
}
