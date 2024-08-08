using FClub.Core.Ioc;
using FClub.Messages.Commands;
using FClub.Messages.Requests;

namespace FClub.Core.Services.FileService;

public interface IFileService : IScopedDependency
{
    Task<string> CombineMp4VideosAsync(string filePath, List<string> urls, CancellationToken cancellationToken);
    
    Task<CombineMp4VideosTaskResponse> CombineMp4VideoTaskAsync(CombineMp4VideosTaskCommand command, CancellationToken cancellationToken);
    
    Task<GetCombineMp4VideosTaskResponse> GetCombineMp4VideoTaskAsync(GetCombineMp4VideosTaskRequest request, CancellationToken cancellationToken);
}