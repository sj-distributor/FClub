using FClub.Messages.Events;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using FClub.Messages.Commands;
using FClub.Core.Services.FileService;

namespace FClub.Core.Handlers.CommandHandlers;

public class CombineMp4VideosCommandHandler : ICommandHandler<CombineMp4VideosCommand, CombineMp4VideosResponse>
{
    private readonly IFileService _fileService;

    public CombineMp4VideosCommandHandler(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    public async Task<CombineMp4VideosResponse> Handle(IReceiveContext<CombineMp4VideosCommand> context, CancellationToken cancellationToken)
    {
        var combinedResult = await _fileService.CombineMp4VideosAsync(context.Message.FilePath, context.Message.Urls, cancellationToken).ConfigureAwait(false);

        var @event = new CombineMp4VideoEvent
        {
            CombinedResult = combinedResult
        };

        await context.PublishAsync(@event, cancellationToken).ConfigureAwait(false);

        return new CombineMp4VideosResponse
        {
            Data = @event.CombinedResult
        };
    }
}