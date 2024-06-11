using FClub.Core.Ioc;
using FClub.Core.Settings.SugarTalk;
using FClub.Messages.Dto.SugarTalk;

namespace FClub.Core.Services.Http.Clients;

public interface ISugarTalkClient : IScopedDependency
{
}

public class SugarTalkClient : ISugarTalkClient
{
    private readonly SugarTalkSettings _sugarTalkSettings;
    private readonly IFClubHttpClientFactory _httpClientFactory;

    public SugarTalkClient(SugarTalkSettings sugarTalkSettings, IFClubHttpClientFactory httpClientFactory)
    {
        _sugarTalkSettings = sugarTalkSettings;
        _httpClientFactory = httpClientFactory;
    }

    public async Task UpdateMeetingRecordUrlAsync(UpdateMeetingRecordUrlDto request, CancellationToken cancellationToken)
    {
        var headers = new Dictionary<string, string>
            {
                {"X-API-KEY", $"{_sugarTalkSettings.ApiKey}"}
            };
        
        await _httpClientFactory.PostAsJsonAsync($"{_sugarTalkSettings.BaseUrl}/Meeting/record/update", request, cancellationToken: cancellationToken, headers: headers).ConfigureAwait(false);
    }
}