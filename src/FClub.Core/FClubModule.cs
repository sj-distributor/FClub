using Amazon;
using Autofac;
using Serilog;
using Amazon.S3;
using Mediator.Net;
using FClub.Core.Ioc;
using FClub.Core.Data;
using System.Reflection;
using Autofac.Core;
using FClub.Core.Settings;
using Mediator.Net.Autofac;
using FClub.Core.Services.Caching;
using Microsoft.EntityFrameworkCore;
using FClub.Core.Middlewares.UnitOfWork;
using Mediator.Net.Middlewares.Serilog;
using Microsoft.Extensions.Configuration;
using FClub.Core.Middlewares.UnifyResponse;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using FClub.Core.Settings.Aws;
using Module = Autofac.Module;

namespace FClub.Core;

public class FClubModule : Module
{
    private readonly ILogger _logger;
    private readonly Assembly[] _assemblies;
    private readonly IConfiguration _configuration;

    public FClubModule(ILogger logger,IConfiguration configuration, params Assembly[] assemblies)
    {
        _logger = logger;
        _assemblies = assemblies;
        _configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        RegisterLogger(builder);
        RegisterSettings(builder);
        RegisterMediator(builder);
        RegisterCaching(builder);
        RegisterDatabase(builder);
        RegisterDependency(builder);
        RegisterAutoMapper(builder);
        RegisterAwsS3Client(builder);
    }

    private void RegisterDependency(ContainerBuilder builder)
    {
        foreach (var type in typeof(IDependency).Assembly.GetTypes()
                     .Where(type => type.IsClass && typeof(IDependency).IsAssignableFrom(type)))
        {
            if (typeof(IScopedDependency).IsAssignableFrom(type))
                builder.RegisterType(type).AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
            else if (typeof(ISingletonDependency).IsAssignableFrom(type))
                builder.RegisterType(type).AsSelf().AsImplementedInterfaces().SingleInstance();
            else if (typeof(ITransientDependency).IsAssignableFrom(type))
                builder.RegisterType(type).AsSelf().AsImplementedInterfaces().InstancePerDependency();
            else
                builder.RegisterType(type).AsSelf().AsImplementedInterfaces();
        }
    }

    private void RegisterMediator(ContainerBuilder builder)
    {
        var mediatorBuilder = new MediatorBuilder();
        
        mediatorBuilder.RegisterHandlers(_assemblies);
        
        mediatorBuilder.ConfigureGlobalReceivePipe(c =>
        {
            c.UseUnitOfWork();
            c.UseUnifyResponse();
            c.UseSerilog(logger: _logger);
        });

        builder.RegisterMediator(mediatorBuilder);
    }
    
    private void RegisterLogger(ContainerBuilder builder)
    {
        builder.RegisterInstance(_logger).AsSelf().AsImplementedInterfaces().SingleInstance();
    }
    
    private void RegisterSettings(ContainerBuilder builder)
    {
        var settingTypes = typeof(FClubModule).Assembly.GetTypes()
            .Where(t => t.IsClass && typeof(IConfigurationSetting).IsAssignableFrom(t))
            .ToArray();

        builder.RegisterTypes(settingTypes).AsSelf().SingleInstance();
    }
    
    private void RegisterDatabase(ContainerBuilder builder)
    {
        builder.RegisterType<FClubDbContext>()
            .AsSelf()
            .As<DbContext>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterType<EfRepository>().As<IRepository>().InstancePerLifetimeScope();
    }
    
    private void RegisterAutoMapper(ContainerBuilder builder)
    {
        builder.RegisterAutoMapper(typeof(FClubModule).Assembly);
    }
    
    private void RegisterCaching(ContainerBuilder builder)
    {
        builder.Register(cfx =>
        {
            var pool = cfx.Resolve<IRedisConnectionPool>();
            return pool.GetConnection();
        }).ExternallyOwned();
    }
    
    private void RegisterAwsS3Client(ContainerBuilder builder)
    {
        builder.Register(c =>
        {
            var settings = c.Resolve<AwsS3Settings>();
            return new AmazonS3Client(settings.AccessKeyId, settings.AccessKeySecret, RegionEndpoint.GetBySystemName(settings.Region));
        }).AsSelf().InstancePerLifetimeScope();
    }
}