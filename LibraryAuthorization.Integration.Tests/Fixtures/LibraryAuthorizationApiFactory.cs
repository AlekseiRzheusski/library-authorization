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

namespace LibraryAuthorization.Integration.Tests.Fixtures;

public class LibraryAuthorizationApiFactory
{
    public HttpClient Client { get; }
    public Mock<IBookGrpcService> BookGrpcServiceMock { get; } = new();

    public LibraryAuthorizationApiFactory()
    {
        var host = new HostBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterInstance(BookGrpcServiceMock.Object)
                       .As<IBookGrpcService>()
                       .SingleInstance();
            })
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services =>
                {
                    services.AddControllers()
                        .AddApplicationPart(typeof(BookController).Assembly);
                });

                webHost.Configure(app =>
                {
                    app.UseMiddleware<RpcExceptionHandlingMiddleware>();

                    app.UseRouting();
                    // app.UseAuthorization();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                });
            })
            .Start();

        Client = host.GetTestClient();
    }
}
