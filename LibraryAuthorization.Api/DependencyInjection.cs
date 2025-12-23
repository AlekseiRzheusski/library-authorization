using Autofac;
using AutoMapper;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using LibraryAuthorization.Api.Interceptors;
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
        builder.RegisterType<AuthenticationClientInterceptor>()
            .SingleInstance();

        builder.Register(ctx =>
        {
            var config = ctx.Resolve<IConfiguration>();
            var interceptor = ctx.Resolve<AuthenticationClientInterceptor>();
            var url = config["Grpc:ServerUrl"];

            var channel = GrpcChannel.ForAddress(url!);

            return channel.Intercept(interceptor);
        }).SingleInstance();

        builder.Register(ctx =>
        {
            var invoker = ctx.Resolve<CallInvoker>();
            return new BookService.BookServiceClient(invoker);
        }).InstancePerLifetimeScope();

        builder.Register(ctx =>
        {
            var invoker = ctx.Resolve<CallInvoker>();
            return new AuthorService.AuthorServiceClient(invoker);
        }).InstancePerLifetimeScope();

        builder.Register(ctx =>
        {
            var invoker = ctx.Resolve<CallInvoker>();
            return new BorrowingService.BorrowingServiceClient(invoker);
        }).InstancePerLifetimeScope();

        //mapper
        builder.Register(ctx =>
        {
            var loggerFactory = ctx.Resolve<ILoggerFactory>(); ;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BookMappingProfile>();
                cfg.AddProfile<AuthorMappingProfile>();
                cfg.AddProfile<BorrowingMappingProfile>();
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
        
        builder.RegisterType<BorrowingGrpcsService>()
            .As<IBorrowingGrpcService>()
            .InstancePerLifetimeScope();

        //repositories
        builder.RegisterType<RefreshTokenRepository>()
            .As<IRefreshTokenRepository>()
            .InstancePerLifetimeScope();
    }
}
