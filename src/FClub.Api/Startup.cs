using Correlate.AspNetCore;
using Serilog;
using FClub.Messages;
using FClub.Api.Extensions;
using Correlate.DependencyInjection;
using FClub.Api.Filters;
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
        services.AddCustomAuthentication(Configuration);
        services.AddCorsPolicy(Configuration);

        services.AddMvc(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        });

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
        app.UseSerilogRequestLogging();
        app.UseCorrelate();
        app.UseRouting();
        app.UseCors();
        app.UseResponseCaching();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHangfireInternal();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("health");
        });
    }
}