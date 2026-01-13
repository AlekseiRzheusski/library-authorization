using LibraryAuthorization.Application.Services.Interfaces;
using LibraryAuthorization.Api.Controllers;
using LibraryAuthorization.Api.Middleware;

using Moq;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.TestHost;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Librarymanagement;

namespace LibraryAuthorization.Integration.Tests.Fixtures;

public class LibraryAuthorizationApiFactory
{
    public HttpClient Client { get; }
    public IServiceProvider Services { get; }
    public Mock<IBookGrpcService> BookGrpcServiceMock { get; } = new();
    public Mock<IBorrowingGrpcService> BorrowingGrpcServiceMock { get; } = new();

    public LibraryAuthorizationApiFactory()
    {
        var host = new HostBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterInstance(BookGrpcServiceMock.Object)
                       .As<IBookGrpcService>()
                       .SingleInstance();

                builder.RegisterInstance(BorrowingGrpcServiceMock.Object)
                       .As<IBorrowingGrpcService>()
                       .SingleInstance();
            })
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services =>
                {
                    services.AddSingleton<TestAuthContext>();
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", _ => { });

                    services.AddAuthorization();

                    services.AddControllers()
                        .AddApplicationPart(typeof(BookController).Assembly);

                    services.AddControllers()
                        .AddApplicationPart(typeof(BorrowingController).Assembly);
                });

                webHost.Configure(app =>
                {
                    app.UseMiddleware<RpcExceptionHandlingMiddleware>();

                    app.UseRouting();

                    app.UseAuthentication();
                    app.UseAuthorization();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            })
            .Start();

        Services = host.Services;
        Client = host.GetTestClient();
    }
}
