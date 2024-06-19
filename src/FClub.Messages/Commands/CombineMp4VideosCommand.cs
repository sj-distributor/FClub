using Mediator.Net.Contracts;
using FClub.Messages.Responses;

namespace FClub.Messages.Commands;

public class CombineMp4VideosCommand : ICommand
{
    public string FilePath { get; set; }
    
    public List<string> Urls { get; set; }
}

public class CombineMp4VideosResponse : FClubResponse<string>
{
}