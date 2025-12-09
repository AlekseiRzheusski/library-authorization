using Autofac;
using LibraryAuthorization.Application.Services;
using LibraryAuthorization.Application.Services.Interfaces;

namespace LibraryAuthorization.Api;

public static class DependencyInjection
{
    public static void RegisterServices(ContainerBuilder builder)
    {
        builder.RegisterType<JwtService>()
            .As<IJwtService>()
            .SingleInstance();
    }
}
