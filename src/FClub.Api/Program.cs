using Autofac;
using Serilog;
using Destructurama;
using FClub.Core.DbUp;
using FClub.Core.Settings;
using FClub.Core.Settings.Logging;
using Autofac.Extensions.DependencyInjection;
using FClub.Core;

namespace FClub.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var apikey = new SerilogApiKeySetting(configuration).Value;
        var serverUrl = new SerilogServerUrlSetting(configuration).Value;
        var application = new SerilogApplicationSetting(configuration).Value;
        
        Log.Logger = new LoggerConfiguration()
            .Destructure.JsonNetTypes()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", application)
            .WriteTo.Console()
            .WriteTo.Seq(serverUrl, apiKey: apikey)
            .CreateLogger();
        
        try
        {
            Log.Information("Configuring api host ({ApplicationContext})...", application);
                
            new DbUpRunner(new FClubConnectionString(configuration).Value).Run();
                
            var webHost = CreateHostBuilder(args, configuration).Build();

            Log.Information("Starting api host ({ApplicationContext})...", application);
                
            webHost.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", application);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureLogging(l => l.AddSerilog(Log.Logger))
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new FClubModule(Log.Logger, configuration, typeof(FClubModule).Assembly));
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}