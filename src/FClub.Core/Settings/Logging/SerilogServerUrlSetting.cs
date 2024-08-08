using Microsoft.Extensions.Configuration;

namespace FClub.Core.Settings.Logging;

public class SerilogServerUrlSetting : IConfigurationSetting<string>
{
    public SerilogServerUrlSetting(IConfiguration configuration)
    {
        Value = configuration.GetValue<string>("Serilog:Seq:ServerUrl");
    }
    
    public string Value { get; set; }
}