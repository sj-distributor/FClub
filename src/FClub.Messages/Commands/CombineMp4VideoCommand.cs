using Mediator.Net.Contracts;
using FClub.Messages.Responses;

namespace FClub.Messages.Commands;

public class CombineMp4VideoCommand : ICommand
{
    public string FilePath { get; set; }
    
    public List<string> Urls { get; set; }
}

public class CombineMp4VideoResponse : FClubResponse<string>
{
}