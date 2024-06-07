using Serilog;
using Newtonsoft.Json;
using FClub.Messages.Enums;
using FClub.Core.Domain.File;
using FClub.Messages.Commands;
using FClub.Messages.Requests;
using File = FClub.Core.Domain.File.File;

namespace FClub.Core.Services.FileService;

public partial class FileService
{
    public async Task<string> CombineMp4VideosAsync(
        string filePath, List<string> urls, CancellationToken cancellationToken)
    {
        try
        {
            Log.Information("Start Loading URL");
            
            var byteArrayList = await ConvertUrlsToByteArraysAsync(urls, cancellationToken).ConfigureAwait(false);

            Log.Information($"CombineMp4VideosAsync byteArrayList: {@byteArrayList}", byteArrayList);
            
            var content = await _ffmpegService.CombineMp4VideosAsync(byteArrayList, cancellationToken).ConfigureAwait(false);

            Log.Information($"CombineMp4VideosAsync content: {content.Length}", content.Length);

            if (content.Length == 0) return "Combine Failed";
            
            var responseUrl = await S3UploadAsync(filePath, content, cancellationToken).ConfigureAwait(false);
            
            return responseUrl;
        }
        catch (Exception ex)
        {
            Log.Error(ex, @"CombineMp4VideosAsync url upload failed, {urls}", JsonConvert.SerializeObject(urls)); 
            throw;
        }
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

        var files = command.Urls.Select(url => new File { Url = url, TaskId = task.Id, Type = FileType.Input, UploadSettingId = fileSetting.Id }).ToList();

        await _fileDataProvider.AddFilesAsync(files, cancellationToken).ConfigureAwait(false);

        _backgroundJobClient.Enqueue(() => ProcessCombineFileTaskAsync(task.Id, command.FilePath, command.Urls, cancellationToken));

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

    public async Task ProcessCombineFileTaskAsync(Guid taskId, string filePath, List<string> urls, CancellationToken cancellationToken)
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
            await CombineTaskAsync(file, filePath, urls, cancellationToken).ConfigureAwait(false);
            await CheckAndUpdateCombineTaskAsync(task, file, cancellationToken).ConfigureAwait(false);
            
        }, cancellationToken).ConfigureAwait(false);
    }
    
    public async Task<string> S3UploadAsync(string filePath, byte[] fileContent, CancellationToken cancellationToken)
    {
        await _awsS3Service.UploadFileAsync(filePath, fileContent, cancellationToken).ConfigureAwait(false);
        
        return await _awsS3Service.GeneratePresignedUrlAsync(filePath).ConfigureAwait(false);
    }

    public async Task<List<byte[]>> ConvertUrlsToByteArraysAsync(List<string> urlList, CancellationToken cancellationToken)
    {
        var byteArrayList = new List<byte[]>();
        
        foreach (var url in urlList)
        {
            try
            {
                var presentTime = _clock.Now;
                
                var data = await DownloadWithRetryAsync(url, 5, cancellationToken);
                
                var downloadTime = _clock.Now - presentTime;
                
                Log.Information("Download completed url: {url}, CONSUME TIME: {@downloadTime}", url, downloadTime);
                
                byteArrayList.Add(data);
            }
            catch (Exception ex)
            {
                Log.Information($"Unable to get data from the URL: {url}, error: {ex.Message}");
            }
        }

        return byteArrayList;
    }

    private async Task<string> GetUrlAsync(string url, CancellationToken cancellationToken)
    {
        if (!url.StartsWith("http"))
            return await _awsS3Service.GeneratePresignedUrlAsync(url, 30).ConfigureAwait(false);
        
        return url;
    }
    
    private async Task<byte[]> DownloadWithRetryAsync(string url, int maxRetries, CancellationToken cancellationToken)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                var uploadUrl = await GetUrlAsync(url, cancellationToken).ConfigureAwait(false);
                
                Log.Information("Uploading url: {url}", uploadUrl);
                
                using (var response = await client.GetAsync(uploadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    await using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        await using (var memoryStream = new MemoryStream())
                        {
                            await contentStream.CopyToAsync(memoryStream, 8192, cancellationToken);
                            return memoryStream.ToArray();
                        }
                    }
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                Log.Warning($"Timeout occurred while downloading {url}, retrying... ({i + 1}/{maxRetries})");
                
                if (i != maxRetries - 1) continue;
                
                Log.Error($"Failed to download {url} after {maxRetries} retries.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while downloading {url}");
                throw;
            }
        }

        return null;
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
    
    public async Task CombineTaskAsync(File file, string filePath, List<string> urls, CancellationToken cancellationToken)
    {
        file.Url = await CombineMp4VideosAsync(filePath, urls, cancellationToken).ConfigureAwait(false);

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
    }
    
    private static readonly HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(300) };
}