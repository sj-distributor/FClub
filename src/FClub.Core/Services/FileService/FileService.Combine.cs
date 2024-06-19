using Serilog;
using Newtonsoft.Json;
using FClub.Messages.Enums;
using FClub.Core.Domain.File;
using FClub.Messages.Commands;
using FClub.Messages.Requests;
using FClub.Messages.Dto.SugarTalk;

namespace FClub.Core.Services.FileService;

public partial class FileService
{
    public async Task<string> CombineMp4VideosAsync(string filePath, List<string> urls, CancellationToken cancellationToken)
    {
        var uploadFileName = "";
        
        try
        {
            var byteArrayList = await ConvertUrlsToByteArraysAsync(urls, cancellationToken).ConfigureAwait(false);

            Log.Information($"CombineMp4VideosAsync byteArrayList: {@byteArrayList}", byteArrayList);

            uploadFileName = await _ffmpegService.CombineMp4VideosAsync(byteArrayList, cancellationToken).ConfigureAwait(false);

            var responseUrl = await S3UploadAsync(filePath, uploadFileName, cancellationToken).ConfigureAwait(false);

            return responseUrl;
        }
        catch (Exception ex)
        {
            Log.Error(ex, @"CombineMp4VideosAsync url upload failed, {urls}", JsonConvert.SerializeObject(urls));
            throw;
        }
        finally
        {
            if (File.Exists(uploadFileName))
                File.Delete(uploadFileName);
        }
    }

    public async Task<CombineMp4VideosTaskResponse> CombineMp4VideoTaskAsync(
        CombineMp4VideosTaskCommand command, CancellationToken cancellationToken)
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

        var files = command.Urls.Select(
            url => new FClubFile { Url = url, TaskId = task.Id, Type = FileType.Input, UploadSettingId = fileSetting.Id }).ToList();

        await _fileDataProvider.AddFilesAsync(files, cancellationToken).ConfigureAwait(false);

        _backgroundJobClient.Enqueue(() => ProcessCombineFileTaskAsync(task.Id, command.UploadId, command.FilePath, command.Urls, cancellationToken));

        return new CombineMp4VideosTaskResponse
        {
            Data = task.Id
        };
    }

    public async Task<GetCombineMp4VideosTaskResponse> GetCombineMp4VideoTaskAsync(
        GetCombineMp4VideosTaskRequest request, CancellationToken cancellationToken)
    {
        var files = await _fileDataProvider.GetFilesAsync(request.TaskId, cancellationToken).ConfigureAwait(false);
        
        return new GetCombineMp4VideosTaskResponse
        {
            Data = _mapper.Map<GetCombineMp4VideoTaskDto>(files.FirstOrDefault(x => x.Type == FileType.Response))
        };
    }

    public async Task ProcessCombineFileTaskAsync(
        Guid taskId, Guid uploadId, string filePath, List<string> urls, CancellationToken cancellationToken)
    {
        var task = await GetCombineTaskAsync(taskId, cancellationToken).ConfigureAwait(false);
        var file = new FClubFile
        {
            TaskId = task.Id,
            Type = FileType.Response
        };
        
        await SafelyCombineFileAsync(task, file, async () =>
        {
            await MarkCombineTaskAsInProgressAsync(task, cancellationToken).ConfigureAwait(false);
            await CombineTaskAsync(file, filePath, urls, cancellationToken).ConfigureAwait(false);
            await UpdateSugarTalkUrlAsync(file, uploadId, cancellationToken).ConfigureAwait(false);
            await CheckAndUpdateCombineTaskAsync(task, file, cancellationToken).ConfigureAwait(false);
            
        }, cancellationToken).ConfigureAwait(false);
    }
    
    private async Task<List<string>> ConvertUrlsToByteArraysAsync(List<string> urlList, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        var byteArrayList = new List<string>();
        
        foreach (var url in urlList)
        {
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var presentTime = _clock.Now;
                
                    var data = await DownloadWithRetryAsync(url, 5, cancellationToken).ConfigureAwait(false);
                
                    var downloadTime = _clock.Now - presentTime;
                
                    Log.Information("Download completed url: {url}, CONSUME TIME: {@downloadTime}", url, downloadTime);
                
                    byteArrayList.Add(data);
                }
                catch (Exception ex)
                {
                    Log.Information($"Unable to get data from the URL: {url}, error: {ex.Message}");
                }
            }, cancellationToken));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        
        return byteArrayList;
    }
    
    private async Task<string> DownloadWithRetryAsync(string url, int maxRetries, CancellationToken cancellationToken)
    {
        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                var temporaryFile = $"{Guid.NewGuid()}.mp4";
                var uploadUrl = await GetUrlAsync(url, cancellationToken).ConfigureAwait(false);
                
                Log.Information("Uploading url: {url}, temporaryFile: {temporaryFile}", uploadUrl, temporaryFile);

                using var response = await client.GetAsync(
                    uploadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                
                response.EnsureSuccessStatusCode();

                await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

                await using var fileStream = new FileStream(temporaryFile, FileMode.Create, FileAccess.Write);
               
                await contentStream.CopyToAsync(fileStream, 8192, cancellationToken).ConfigureAwait(false);

                return temporaryFile;
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

        return null!;
    }
    
    private async Task<string> S3UploadAsync(string filePath, string uploadFile, CancellationToken cancellationToken)
    {
        await _awsS3Service.UploadFileToS3StreamAsync(filePath, uploadFile, cancellationToken).ConfigureAwait(false);
        
        return filePath;
    }

    private async Task<string> GetUrlAsync(string url, CancellationToken cancellationToken)
    {
        if (!url.StartsWith("http"))
            return await _awsS3Service.GeneratePresignedUrlAsync(url, 30).ConfigureAwait(false);
        
        return url;
    }
    
    private async Task MarkFileTaskAsFailedAsync(FileTask task, CancellationToken cancellationToken)
    {
        task.Status = FileTaskStatus.Failed;
        
        await _fileDataProvider.UpdateFileTaskAsync(task, cancellationToken).ConfigureAwait(false);
    }
    
    private async Task MarkCombineTaskAsInProgressAsync(FileTask task, CancellationToken cancellationToken)
    {
        task.Status = FileTaskStatus.Processing;
        
        await _fileDataProvider.UpdateFileTaskAsync(task, cancellationToken).ConfigureAwait(false);
    }
    
    private async Task CombineTaskAsync(FClubFile fClubFile, string filePath, List<string> urls, CancellationToken cancellationToken)
    {
        fClubFile.Url = await CombineMp4VideosAsync(filePath, urls, cancellationToken).ConfigureAwait(false);

        await _fileDataProvider.AddFileAsync(fClubFile, cancellationToken).ConfigureAwait(false);
    }

    private async Task UpdateSugarTalkUrlAsync(FClubFile fClubFile, Guid id, CancellationToken cancellationToken)
    {
        if (fClubFile.Url == null)
            return;
        
        Log.Information($"Update SugartTalk url argument：url: {fClubFile.Url}, id: {id}", fClubFile.Url, id);
        
        await _sugarTalkClient.UpdateMeetingRecordUrlAsync(
            new UpdateMeetingRecordUrlDto{ Id = id, Url = fClubFile.Url}, cancellationToken).ConfigureAwait(false);
    }

    private async Task<FileTask> GetCombineTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        return await _fileDataProvider.GetFileTaskByIdAsync(taskId, cancellationToken).ConfigureAwait(false);
    }
    
    private async Task CheckAndUpdateCombineTaskAsync(FileTask task, FClubFile fClubFile, CancellationToken cancellationToken)
    {
        task.Status = !string.IsNullOrEmpty(fClubFile.Url) ? FileTaskStatus.Success : FileTaskStatus.Failed;
        
        await _fileDataProvider.UpdateFileTaskAsync(task, cancellationToken).ConfigureAwait(false);
    }

    private async Task SafelyCombineFileAsync(FileTask task, FClubFile fClubFile, Func<Task> action, CancellationToken cancellationToken)
    {
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