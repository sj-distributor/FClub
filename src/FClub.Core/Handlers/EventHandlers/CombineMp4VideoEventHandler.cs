using FClub.Messages.Events;
using Mediator.Net.Context;
using Mediator.Net.Contracts;

namespace FClub.Core.Handlers.EventHandlers;

public class CombineMp4VideoEventHandler : IEventHandler<CombineMp4VideoEvent>
{
    public Task Handle(IReceiveContext<CombineMp4VideoEvent> context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}