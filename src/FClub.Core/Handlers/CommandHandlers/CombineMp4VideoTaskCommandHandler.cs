using FClub.Messages.Commands;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using FClub.Core.Services.FileService;

namespace FClub.Core.Handlers.CommandHandlers;

public class CombineMp4VideoTaskCommandHandler : ICommandHandler<CombineMp4VideoTaskCommand, CombineMp4VideoTaskResponse>
{
    private readonly IFileService _fileService;

    public CombineMp4VideoTaskCommandHandler(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<CombineMp4VideoTaskResponse> Handle(IReceiveContext<CombineMp4VideoTaskCommand> context, CancellationToken cancellationToken)
    {
        /*return await _fileService.CombineMp4VideoTaskAsync(context.Message, cancellationToken).ConfigureAwait(false);*/

        return new CombineMp4VideoTaskResponse();
    }
}