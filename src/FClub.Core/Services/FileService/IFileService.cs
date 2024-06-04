using FClub.Core.Ioc;
using FClub.Messages.Commands;
using FClub.Messages.Requests;

namespace FClub.Core.Services.FileService;

public interface IFileService : IScopedDependency
{
    Task<string> CombineMp4VideosAsync(
        string filePath, List<string> urls, CancellationToken cancellationToken);
    
    Task<CombineMp4VideoTaskResponse> CombineMp4VideoTaskAsync(CombineMp4VideoTaskCommand command, CancellationToken cancellationToken);
    
    Task<GetCombineMp4VideoTaskResponse> GetCombineMp4VideoTaskAsync(GetCombineMp4VideoTaskRequest request, CancellationToken cancellationToken);
}