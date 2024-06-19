using Microsoft.Extensions.Configuration;

namespace FClub.Core.Settings.SugarTalk;

public class SugarTalkSettings : IConfigurationSetting
{
    public SugarTalkSettings(IConfiguration configuration)
    {
        ApiKey = configuration.GetValue<string>("SugarTalk:ApiKey");
        BaseUrl = configuration.GetValue<string>("SugarTalk:BaseUrl");
    }

    public string ApiKey { get; set; }

    public string BaseUrl { get; set; }
}