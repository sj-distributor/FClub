using Mediator.Net.Context;
using Mediator.Net.Contracts;
using FClub.Messages.Requests;
using FClub.Core.Services.FileService;

namespace FClub.Core.Handlers.RequestHandlers;

public class GetCombineMp4VideosTaskRequestHandler : IRequestHandler<GetCombineMp4VideosTaskRequest, GetCombineMp4VideosTaskResponse>
{
    private readonly IFileService _fileService;

    public GetCombineMp4VideosTaskRequestHandler(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task<GetCombineMp4VideosTaskResponse> Handle(IReceiveContext<GetCombineMp4VideosTaskRequest> context, CancellationToken cancellationToken)
    {
        return await _fileService.GetCombineMp4VideoTaskAsync(context.Message, cancellationToken).ConfigureAwait(false);
    }
}