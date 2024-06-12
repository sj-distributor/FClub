using Mediator.Net.Contracts;

namespace FClub.Messages.Events;

public class CombineMp4VideoEvent : IEvent
{
    public string CombinedResult { get; set; }
}