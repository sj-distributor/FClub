using FClub.Core.Ioc;
using FClub.Core.Domain.File;

namespace FClub.Core.Services.FileService;

public interface IFileDataProvider : IScopedDependency
{
    Task AddFileTaskAsync(FileTask fileTask, bool forSave = true, CancellationToken cancellationToken = default);

    Task AddFilesAsync(List<FClubFile> fClubFiles, bool forSave = true, CancellationToken cancellationToken = default);

    Task AddFileAsync(FClubFile fClubFile, bool forSave = true, CancellationToken cancellationToken = default);
    
    Task AddUploadSettingAsync(UploadSetting uploadSetting, bool forSave = true, CancellationToken cancellationToken = default);

    Task UpdateFileTaskAsync(FileTask fileTask, bool forSave = true, CancellationToken cancellationToken = default);

    Task<FileTask> GetFileTaskByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<List<FClubFile>> GetFilesAsync(Guid? taskId, CancellationToken cancellationToken);
}