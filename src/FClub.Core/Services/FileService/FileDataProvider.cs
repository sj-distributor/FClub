using FClub.Core.Data;
using FClub.Core.Domain.File;
using Microsoft.EntityFrameworkCore;
using File = FClub.Core.Domain.File.File;

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

    public async Task AddFileTaskAsync(FileTask fileTask, CancellationToken cancellationToken)
    {
        await _repository.InsertAsync(fileTask, cancellationToken).ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddFilesAsync(List<File> files, CancellationToken cancellationToken)
    {
        await _repository.InsertAllAsync(files, cancellationToken).ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
    
    public async Task AddFileAsync(File file, CancellationToken cancellationToken)
    {
        await _repository.InsertAsync(file, cancellationToken).ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddUploadSettingAsync(UploadSetting uploadSetting, CancellationToken cancellationToken)
    {
        await _repository.InsertAsync(uploadSetting, cancellationToken).ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateFileTaskAsync(FileTask fileTask, CancellationToken cancellationToken)
    {
        await _repository.UpdateAsync(fileTask, cancellationToken).ConfigureAwait(false);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<FileTask> GetFileTaskByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync<FileTask>(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<File>> GetFilesAsync(Guid? taskId, CancellationToken cancellationToken)
    {
        var qeury = _repository.Query<File>();

        if (taskId.HasValue)
            qeury = qeury.Where(x => x.TaskId == taskId.Value);

        return await qeury.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}