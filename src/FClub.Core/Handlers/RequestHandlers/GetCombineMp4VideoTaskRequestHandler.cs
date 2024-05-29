using FClub.Messages.Requests;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using FClub.Core.Services.FileService;

namespace FClub.Core.Handlers.RequestHandlers;

public class GetCombineMp4VideoTaskRequestHandler : IRequestHandler<GetCombineMp4VideoTaskRequest, GetCombineMp4VideoTaskResponse>
{
    private readonly IFileService _fileService;

    public GetCombineMp4VideoTaskRequestHandler(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<GetCombineMp4VideoTaskResponse> Handle(IReceiveContext<GetCombineMp4VideoTaskRequest> context, CancellationToken cancellationToken)
    {
        return await _fileService.GetCombineMp4VideoTaskAsync(context.Message, cancellationToken).ConfigureAwait(false);
    }
}