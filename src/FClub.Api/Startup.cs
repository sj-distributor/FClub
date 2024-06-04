using FClub.Messages;
using FClub.Api.Extensions;
using Correlate.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace FClub.Api;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCorrelate(options => options.RequestHeaders = FClubConstants.CorrelationIdHeaders);
        services.AddControllers().AddNewtonsoftJson();
        services.AddHttpClientInternal();
        services.AddMemoryCache();
        services.AddResponseCaching();
        services.AddHealthChecks();
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddCustomSwagger();
        services.AddCorsPolicy(Configuration);
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FClub.Api.xml");
                c.DocExpansion(DocExpansion.None);
            });
        }

        app.UseRouting();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}