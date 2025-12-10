using Autofac;
using AutoMapper;
using Grpc.Net.Client;
using LibraryAuthorization.Application.Mappers;
using LibraryAuthorization.Application.Services;
using LibraryAuthorization.Application.Services.Interfaces;
using Librarymanagement;

namespace LibraryAuthorization.Api;

public static class DependencyInjection
{
    public static void RegisterServices(ContainerBuilder builder)
    {
        //grpc
        builder.Register(ctx =>
        {
            var config = ctx.Resolve<IConfiguration>();
            var url = config["Grpc:BookServiceUrl"];

            return GrpcChannel.ForAddress(url!);
        }).SingleInstance();

        builder.Register(ctx =>
        {
            var channel = ctx.Resolve<GrpcChannel>();
            return new BookService.BookServiceClient(channel);
        }).InstancePerLifetimeScope();

        //mapper
        builder.Register(ctx =>
        {
            var loggerFactory = ctx.Resolve<ILoggerFactory>();;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BookMappingProfile>();
            }, loggerFactory);

            return config.CreateMapper();
        })
        .As<IMapper>()
        .SingleInstance();

        //services
        builder.RegisterType<JwtService>()
            .As<IJwtService>()
            .SingleInstance();
        
        builder.RegisterType<BookGrpcService>()
            .As<IBookGrpcService>()
            .InstancePerLifetimeScope();
    }
}
