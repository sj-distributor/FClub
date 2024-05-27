using Autofac;
using Serilog;
using Mediator.Net;
using System.Reflection;
using FClub.Core.Ioc;
using FClub.Core.Middlewares.UnitOfWork;
using FClub.Core.Settings;
using Mediator.Net.Autofac;
using Mediator.Net.Middlewares.Serilog;
using Microsoft.Extensions.Configuration;
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
        RegisterMediator(builder);
        RegisterDependency(builder);
        RegisterSettings(builder);
        RegisterLogger(builder);
    }

    private void RegisterDependency(ContainerBuilder builder)
    {
        foreach (var type in typeof(IDependency).Assembly.GetTypes()
                     .Where(t => typeof(IDependency).IsAssignableFrom(t) && t.IsClass))
        {
            switch (type)
            {
                case var t when typeof(IScopedDependency).IsAssignableFrom(type):
                    builder.RegisterType(t).AsImplementedInterfaces().InstancePerLifetimeScope();
                    break;
                case var t when typeof(ISingletonDependency).IsAssignableFrom(type):
                    builder.RegisterType(t).AsImplementedInterfaces().SingleInstance();
                    break;
                case var t when typeof(ITransientDependency).IsAssignableFrom(type):
                    builder.RegisterType(t).AsImplementedInterfaces().InstancePerDependency();
                    break;
                default:
                    builder.RegisterType(type).AsImplementedInterfaces();
                    break;
            }
        }
    }

    private void RegisterMediator(ContainerBuilder builder)
    {
        var mediatorBuilder = new MediatorBuilder();
        
        mediatorBuilder.RegisterHandlers(_assemblies);
        
        mediatorBuilder.ConfigureGlobalReceivePipe(c =>
        {
            c.UseUnitOfWork();
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
}