using Mediator.Net.Context;
using Mediator.Net.Contracts;
using FClub.Messages.Commands;
using FClub.Messages.Events;
using FClub.Core.Services.FileService;

namespace FClub.Core.Handlers.CommandHandlers;

public class CombineMp4VideoCommandHandler : ICommandHandler<CombineMp4VideoCommand, CombineMp4VideoResponse>
{
    private readonly IFileService _fileService;

    public CombineMp4VideoCommandHandler(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    public async Task<CombineMp4VideoResponse> Handle(IReceiveContext<CombineMp4VideoCommand> context, CancellationToken cancellationToken)
    {
        var combinedResult = await _fileService.CombineMp4VideosAsync(context.Message.FilePath, context.Message.S3UploadDto, context.Message.Urls, cancellationToken).ConfigureAwait(false);

        var @event = new CombineMp4VideoEvent
        {
            CombinedResult = combinedResult
        };

        await context.PublishAsync(@event, cancellationToken).ConfigureAwait(false);

        return new CombineMp4VideoResponse
        {
            Data = @event.CombinedResult
        };
    }
}