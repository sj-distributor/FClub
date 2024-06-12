using Amazon;
using Serilog;
using Amazon.S3;
using FClub.Messages.Enums;
using FClub.Core.Domain.File;
using FClub.Messages.Commands;
using FClub.Messages.Requests;
using FClub.Messages.Dto.Upload;
using File = FClub.Core.Domain.File.File;

namespace FClub.Core.Services.FileService;

public partial class FileService
{
    /*public async Task<string> CombineMp4VideosAsync(
        string filePath, S3UploadDto s3UploadDto, List<string> urls, CancellationToken cancellationToken)
    {
        var byteArrayList = await ConvertUrlsToByteArrays(urls);

        var content = await _ffmpegService.CombineMp4VideosAsync(byteArrayList, cancellationToken).ConfigureAwait(false);

        var url = await S3UploadAsync(s3UploadDto, filePath, content, cancellationToken).ConfigureAwait(false);

        return url;
    }

    public async Task<CombineMp4VideoTaskResponse> CombineMp4VideoTaskAsync(
        CombineMp4VideoTaskCommand command, CancellationToken cancellationToken)
    {
        var task = new FileTask
        {
            Id = command.TaskId ?? Guid.NewGuid(),
            Status = FileTaskStatus.Pending
        };

        var fileSetting = new UploadSetting
        {
            FilePath = command.FilePath,
            UploadAddressType = UploadType.Aws
        };

        await _fileDataProvider.AddUploadSettingAsync(fileSetting, cancellationToken).ConfigureAwait(false);
        
        await _fileDataProvider.AddFileTaskAsync(task, cancellationToken).ConfigureAwait(false);

        var files = command.Urls.Select(url => new File { Url = url, TaskId = task.Id, Type = FileType.Input, CompletedSettingId = fileSetting.Id }).ToList();

        await _fileDataProvider.AddFilesAsync(files, cancellationToken).ConfigureAwait(false);

        _backgroundJobClient.Enqueue(() => ProcessCombineFileTaskAsync(task.Id, command.S3UploadDto, command.FilePath, command.Urls, cancellationToken));

        return new CombineMp4VideoTaskResponse
        {
            Data = task.Id
        };
    }

    public async Task<GetCombineMp4VideoTaskResponse> GetCombineMp4VideoTaskAsync(
        GetCombineMp4VideoTaskRequest request, CancellationToken cancellationToken)
    {
        var files = await _fileDataProvider.GetFilesAsync(request.TaskId, cancellationToken).ConfigureAwait(false);
        
        return new GetCombineMp4VideoTaskResponse
        {
            Data = files.Select(x => x.Url).ToList()
        };
    }

    public async Task ProcessCombineFileTaskAsync(Guid taskId, S3UploadDto s3Upload, string filePath, List<string> urls, CancellationToken cancellationToken)
    {
        var task = await GetCombineTaskAsync(taskId, cancellationToken).ConfigureAwait(false);
        var file = new File
        {
            TaskId = task.Id,
            Type = FileType.Response
        };
        
        await SafelyCombineFileAsync(task, file, async () =>
        {
            await MarkCombineTaskAsInProgressAsync(task, cancellationToken).ConfigureAwait(false);
            await CombineTaskAsync(file, filePath, s3Upload, urls, cancellationToken).ConfigureAwait(false);
            await CheckAndUpdateCombineTaskAsync(task, file, cancellationToken).ConfigureAwait(false);
            
        }, cancellationToken).ConfigureAwait(false);
    }
    
    public async Task<string> S3UploadAsync(S3UploadDto s3UploadDto, string filePath, byte[] fileContent, CancellationToken cancellationToken)
    {
        var amazonS3Client = new AmazonS3Client(s3UploadDto.AccessKey, s3UploadDto.Secret, RegionEndpoint.GetBySystemName(s3UploadDto.Region)); 
        
        await _awsS3Service.UploadFileAsync(amazonS3Client, s3UploadDto.Bucket, filePath, fileContent, cancellationToken).ConfigureAwait(false);
        
        return await _awsS3Service.GeneratePresignedUrlAsync(amazonS3Client, s3UploadDto.Bucket, filePath).ConfigureAwait(false);
    }

    public static async Task<List<byte[]>> ConvertUrlsToByteArrays(List<string> urlList)
    {
        var byteArrayList = new List<byte[]>();

        using var client = new HttpClient();
        
        foreach (var url in urlList)
        {
            try
            {
                var data = await client.GetByteArrayAsync(url);
    
                byteArrayList.Add(data);
            }
            catch (Exception ex)
            {
                Log.Information($"Unable to get data from the URL: {url}, error: {ex.Message}");
            }
        }

        return byteArrayList;
    }
    
    public async Task MarkFileTaskAsFailedAsync(FileTask task, CancellationToken cancellationToken)
    {
        task.Status = FileTaskStatus.Failed;
        
        await _fileDataProvider.UpdateFileTaskAsync(task, cancellationToken).ConfigureAwait(false);
    }
    
    public async Task MarkCombineTaskAsInProgressAsync(FileTask task, CancellationToken cancellationToken)
    {
        task.Status = FileTaskStatus.Processing;
        
        await _fileDataProvider.UpdateFileTaskAsync(task, cancellationToken).ConfigureAwait(false);
    }
    
    public async Task CombineTaskAsync(File file, string filePath, S3UploadDto s3UploadDto, List<string> urls, CancellationToken cancellationToken)
    {
        file.Url = await CombineMp4VideosAsync(filePath, s3UploadDto, urls, cancellationToken).ConfigureAwait(false);

        await _fileDataProvider.AddFileAsync(file, cancellationToken).ConfigureAwait(false);
    }

    public async Task<FileTask> GetCombineTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        return await _fileDataProvider.GetFileTaskByIdAsync(taskId, cancellationToken).ConfigureAwait(false);
    }
    
    public async Task CheckAndUpdateCombineTaskAsync(FileTask task, File file, CancellationToken cancellationToken)
    {
        task.Status = !string.IsNullOrEmpty(file.Url) ? FileTaskStatus.Success : FileTaskStatus.Failed;
        
        await _fileDataProvider.UpdateFileTaskAsync(task, cancellationToken).ConfigureAwait(false);
    }

    public async Task SafelyCombineFileAsync(FileTask task, File file, Func<Task> action, CancellationToken cancellationToken)
    {
        if (task == null) return;
        if (file == null) return;
        
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            await action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await MarkFileTaskAsFailedAsync(task, cancellationToken).ConfigureAwait(false);
            
            Log.Error(ex, "Error on translation");
        }
    }*/
}