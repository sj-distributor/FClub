using FClub.Core.Ioc;
using FClub.Core.Domain.File;
using File = FClub.Core.Domain.File.File;

namespace FClub.Core.Services.FileService;

public interface IFileDataProvider : IScopedDependency
{
    Task AddFileTaskAsync(FileTask fileTask, CancellationToken cancellationToken);

    Task AddFilesAsync(List<File> files, CancellationToken cancellationToken);

    Task AddFileAsync(File file, CancellationToken cancellationToken);
    
    Task AddUploadSettingAsync(UploadSetting uploadSetting, CancellationToken cancellationToken);

    Task UpdateFileTaskAsync(FileTask fileTask, CancellationToken cancellationToken);

    Task<FileTask> GetFileTaskByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<List<File>> GetFilesAsync(Guid? taskId, CancellationToken cancellationToken);
}