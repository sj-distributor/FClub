using FClub.Core.Ioc;
using FClub.Core.Domain.File;

namespace FClub.Core.Services.FileService;

public interface IFileDataProvider : IScopedDependency
{
    Task AddFileTaskAsync(FileTask fileTask, CancellationToken cancellationToken);

    Task AddFilesAsync(List<FClubFile> files, CancellationToken cancellationToken);

    Task AddFileAsync(FClubFile fClubFile, CancellationToken cancellationToken);
    
    Task AddUploadSettingAsync(UploadSetting uploadSetting, CancellationToken cancellationToken);

    Task UpdateFileTaskAsync(FileTask fileTask, CancellationToken cancellationToken);

    Task<FileTask> GetFileTaskByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<List<FClubFile>> GetFilesAsync(Guid? taskId, CancellationToken cancellationToken);
}