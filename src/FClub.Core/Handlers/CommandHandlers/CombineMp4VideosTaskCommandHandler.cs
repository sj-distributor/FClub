using Mediator.Net.Context;
using Mediator.Net.Contracts;
using FClub.Messages.Commands;
using FClub.Core.Services.FileService;

namespace FClub.Core.Handlers.CommandHandlers;

public class CombineMp4VideosTaskCommandHandler : ICommandHandler<CombineMp4VideosTaskCommand, CombineMp4VideosTaskResponse>
{
    private readonly IFileService _fileService;

    public CombineMp4VideosTaskCommandHandler(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<CombineMp4VideosTaskResponse> Handle(IReceiveContext<CombineMp4VideosTaskCommand> context, CancellationToken cancellationToken)
    {
        return await _fileService.CombineMp4VideoTaskAsync(context.Message, cancellationToken).ConfigureAwait(false);
    }
}