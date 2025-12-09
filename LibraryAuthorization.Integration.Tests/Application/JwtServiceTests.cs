using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Autofac;
using LibraryAuthorization.Application.Services;
using LibraryAuthorization.Application.Services.Interfaces;
using LibraryAuthorization.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace LibraryAuthorization.Integration.Tests.Application;

public class JwtServiceTests
{
    private readonly IContainer _container;
    private readonly LibraryUser _testUser;

    public JwtServiceTests()
    {
        var builder = new ContainerBuilder();
        var configMock = new Mock<IConfiguration>();

        configMock.Setup(c => c["Jwt:Key"]).Returns("SecretKey_123456789123456789123456789");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        builder.RegisterInstance(configMock.Object).As<IConfiguration>();

        var userStoreMock = new Mock<IUserStore<LibraryUser>>();
        var userManagerMock = new Mock<UserManager<LibraryUser>>(
            userStoreMock.Object,
            Mock.Of<Microsoft.Extensions.Options.IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<LibraryUser>>(),
            new IUserValidator<LibraryUser>[0],
            new IPasswordValidator<LibraryUser>[0],
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<LibraryUser>>>()
        );

        _testUser = new LibraryUser { UserName = "testuser" };
        userManagerMock.Setup(um => um.GetRolesAsync(_testUser))
                       .ReturnsAsync(new List<string> { "User" });

        builder.RegisterInstance(userManagerMock.Object).As<UserManager<LibraryUser>>();

        builder.RegisterType<JwtService>()
            .As<IJwtService>()
            .InstancePerLifetimeScope();

        _container = builder.Build();
    }

    [Fact]
    public async Task GenerateJwtToken_ShouldReturnJwtToken()
    {
        using var scope = _container.BeginLifetimeScope();

        var jwtService = scope.Resolve<IJwtService>();
        var configuration = scope.Resolve<IConfiguration>();

        var token = await jwtService.GenerateJwtToken(_testUser);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.False(string.IsNullOrEmpty(token));
        Assert.Equal(_testUser.UserName, jwt.Subject);
        Assert.Equal(configuration["Jwt:Issuer"], jwt.Issuer);
    }
}
