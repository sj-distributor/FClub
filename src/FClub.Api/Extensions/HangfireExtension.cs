using Serilog;
using Hangfire;
using Newtonsoft.Json;
using FClub.Core.Jobs;
using Hangfire.Correlate;
using FClub.Core.Constants;
using FClub.Core.Services.Jobs;
using FClub.Core.Settings.Caching;

namespace FClub.Api.Extensions;

public static class HangfireExtension
{
    public static void AddHangfireInternal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire((sp, c) =>
        {
            c.UseCorrelate(sp);
            c.UseMaxArgumentSizeToRender(int.MaxValue);
            c.UseFilter(new AutomaticRetryAttribute { Attempts = 0 });
            c.UseRedisStorage(new RedisCacheConnectionStringSetting(configuration).Value);
            c.UseSerializerSettings(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        });

        services.AddHangfireServer(opt =>
        {
            opt.ServerTimeout = TimeSpan.FromHours(2);
            opt.Queues = new[]
            {
                HangfireConstants.DefaultQueue,
                HangfireConstants.MeetingSummaryQueue
            };
        });
    }
}