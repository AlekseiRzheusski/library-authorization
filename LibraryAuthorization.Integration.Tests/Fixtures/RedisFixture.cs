using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using StackExchange.Redis;

namespace LibraryAuthorization.Integration.Tests.Fixtures;

public class RedisFixture : IAsyncLifetime
{
    private TestcontainersContainer _container = null!;

    public IConnectionMultiplexer Redis { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _container = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("redis:7")
            .WithName($"redis-test-{Guid.NewGuid()}")
            .WithPortBinding(6379, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();
        
        await _container.StartAsync();

        var host = _container.Hostname;
        var port = _container.GetMappedPublicPort(6379);

        Redis = await ConnectionMultiplexer.ConnectAsync(
            $"{host}:{port}");
    }

    public async Task DisposeAsync()
    {
        if (Redis != null)
            await Redis.CloseAsync();

        await _container.DisposeAsync();
    }

}
