using FClub.Core.Data;
using FClub.Core.Domain.File;
using Microsoft.EntityFrameworkCore;

namespace FClub.Core.Services.FileService;

public class FileDataProvider : IFileDataProvider
{
    private readonly IRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public FileDataProvider(IRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task AddFileTaskAsync(FileTask fileTask, bool forSave = true, CancellationToken cancellationToken = default)
    {
        await _repository.InsertAsync(fileTask, cancellationToken).ConfigureAwait(false);

        if (forSave)
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddFilesAsync(List<FClubFile> fClubFiles, bool forSave = true, CancellationToken cancellationToken = default)
    {
        await _repository.InsertAllAsync(fClubFiles, cancellationToken).ConfigureAwait(false);

        if (forSave)
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
    
    public async Task AddFileAsync(FClubFile fClubFile, bool forSave = true, CancellationToken cancellationToken = default)
    {
        await _repository.InsertAsync(fClubFile, cancellationToken).ConfigureAwait(false);

        if (forSave)
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddUploadSettingAsync(UploadSetting uploadSetting, bool forSave = true, CancellationToken cancellationToken = default)
    {
        await _repository.InsertAsync(uploadSetting, cancellationToken).ConfigureAwait(false);

        if (forSave)
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateFileTaskAsync(FileTask fileTask, bool forSave = true, CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(fileTask, cancellationToken).ConfigureAwait(false);

        if (forSave)
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<FileTask> GetFileTaskByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync<FileTask>(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<FClubFile>> GetFilesAsync(Guid? taskId, CancellationToken cancellationToken)
    {
        var qeury = _repository.Query<FClubFile>();

        if (taskId.HasValue)
            qeury = qeury.Where(x => x.TaskId == taskId.Value);

        return await qeury.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}