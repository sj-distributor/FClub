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
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddResponseCaching();
        services.AddEndpointsApiExplorer();
        services.AddCustomSwagger();
        services.AddMvc();
        services.AddControllers();
        services.AddHangfireInternal(Configuration);
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