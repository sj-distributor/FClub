using Microsoft.Extensions.Configuration;

namespace FClub.Core.Settings;

public class FClubConnectionString : IConfigurationSetting<string>
{
    public FClubConnectionString(IConfiguration configuration)
    {
        Value = configuration.GetConnectionString("FClubConnectionString");
    }

    public string Value { get; set; }
}