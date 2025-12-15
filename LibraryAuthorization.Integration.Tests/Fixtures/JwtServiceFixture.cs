using LibraryAuthorization.Application.Services.Interfaces;
using LibraryAuthorization.Application.Services;
using LibraryAuthorization.Infrastructure.Repositories.Interfaces;
using LibraryAuthorization.Infrastructure.Repositories;

using Autofac;
using Moq;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using LibraryAuthorization.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LibraryAuthorization.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;

namespace LibraryAuthorization.Integration.Tests.Fixtures;

public class JwtServiceFixture : IAsyncLifetime
{
    public IContainer Container { get; private set; } = null!;
    private SqliteConnection _connection = null!;
    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseSqlite(_connection)
            .Options;

        var services = new ServiceCollection();
        var configMock = new Mock<IConfiguration>();

        configMock.Setup(c => c["Jwt:Key"]).Returns("SecretKey_123456789123456789123456789");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");

        services.AddDbContext<AuthDbContext>(opt => opt.UseSqlite(_connection));

        services.AddIdentityCore<LibraryUser>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequiredLength = 1;
        })
        .AddRoles<LibraryRole>()
        .AddEntityFrameworkStores<AuthDbContext>();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        var builder = new ContainerBuilder();

        builder.RegisterInstance(configMock.Object)
            .As<IConfiguration>();

        builder.Populate(services);

        Container = builder.Build();

        using (var scope = Container.BeginLifetimeScope())
        {
            var context = scope.Resolve<AuthDbContext>();
            await context.Database.MigrateAsync();
        }
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        Container.Dispose();
    }
}
