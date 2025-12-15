using Autofac;
using AutoMapper;
using Grpc.Net.Client;
using LibraryAuthorization.Application.Mappers;
using LibraryAuthorization.Application.Services;
using LibraryAuthorization.Application.Services.Interfaces;
using LibraryAuthorization.Domain.Entities;
using LibraryAuthorization.Infrastructure.Repositories;
using LibraryAuthorization.Infrastructure.Repositories.Interfaces;
using Librarymanagement;

namespace LibraryAuthorization.Api;

public class DependencyInjection : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        //grpc
        builder.Register(ctx =>
        {
            var config = ctx.Resolve<IConfiguration>();
            var url = config["Grpc:ServerUrl"];

            return GrpcChannel.ForAddress(url!);
        }).SingleInstance();

        builder.Register(ctx =>
        {
            var channel = ctx.Resolve<GrpcChannel>();
            return new BookService.BookServiceClient(channel);
        }).InstancePerLifetimeScope();

        builder.Register(ctx =>
        {
            var channel = ctx.Resolve<GrpcChannel>();
            return new AuthorService.AuthorServiceClient(channel);
        }).InstancePerLifetimeScope();

        //mapper
        builder.Register(ctx =>
        {
            var loggerFactory = ctx.Resolve<ILoggerFactory>();;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BookMappingProfile>();
                cfg.AddProfile<AuthorMappingProfile>();
            }, loggerFactory);

            return config.CreateMapper();
        })
        .As<IMapper>()
        .SingleInstance();

        //services
        builder.RegisterType<JwtService>()
            .As<IJwtService>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<BookGrpcService>()
            .As<IBookGrpcService>()
            .InstancePerLifetimeScope();
        
        builder.RegisterType<AuthorGrpcService>()
            .As<IAuthorGrpcService>()
            .InstancePerLifetimeScope();

        //repositories
        builder.RegisterType<RefreshTokenRepository>()
            .As<IRefreshTokenRepository>()
            .InstancePerLifetimeScope();
    }
}
