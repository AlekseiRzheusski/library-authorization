using LibraryAuthorization.Application.Services.Interfaces;
using LibraryAuthorization.Domain.Entities;
using LibraryAuthorization.Infrastructure.Repositories.Interfaces;
using LibraryAuthorization.Integration.Tests.Fixtures;

using Autofac;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace LibraryAuthorization.Integration.Tests.Application;

public class JwtServiceTests : IClassFixture<JwtServiceFixture>
{
    private readonly JwtServiceFixture _fixture;

    public JwtServiceTests(JwtServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GenerateJwtToken_ShouldReturnJwtToken()
    {
        using var scope = _fixture.Container.BeginLifetimeScope();

        var jwtService = scope.Resolve<IJwtService>();
        var configuration = scope.Resolve<IConfiguration>();
        var userManager = scope.Resolve<UserManager<LibraryUser>>();
        var repository = scope.Resolve<IRefreshTokenRepository>();

        var user = new LibraryUser
        {
            UserName = "test",
        };
        var result = await userManager.CreateAsync(user, "Password123!");
        await userManager.AddToRoleAsync(user, "User");

        var token = await jwtService.GenerateJwtToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token.AccessToken);

        Assert.False(string.IsNullOrEmpty(token.AccessToken));
        Assert.False(string.IsNullOrEmpty(token.RefreshToken));

        Assert.Equal(user.Id, long.Parse((string)jwt.Subject));

        var name = jwt.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        Assert.Equal(user.UserName, name);

        Assert.Equal(configuration["Jwt:Issuer"], jwt.Issuer);

        var refreshToken = await repository.FindDetailedEntityByToken(token.RefreshToken);

        Assert.NotNull(refreshToken);
        Assert.False(refreshToken.IsRevoked);
    }
}
