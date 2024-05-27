using FClub.Core.Services;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using FClub.Messages.Commands;

namespace FClub.Core.Handlers.CommandHandlers;

public class CombineMp4VideoCommandHandler : ICommandHandler<CombineMp4VideoCommand, CombineMp4VideoResponse>
{
    private readonly IFfmpegService _ffmpegService;

    public CombineMp4VideoCommandHandler(IFfmpegService ffmpegService)
    {
        _ffmpegService = ffmpegService;
    }
    
    public async Task<CombineMp4VideoResponse> Handle(IReceiveContext<CombineMp4VideoCommand> context, CancellationToken cancellationToken)
    {
        return await _ffmpegService.CombineMp4VideosAsync(context.Message, cancellationToken).ConfigureAwait(false);
    }
}