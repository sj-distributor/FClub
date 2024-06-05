using Serilog;
using FClub.Core.Ioc;
using System.Diagnostics;

namespace FClub.Core.Services.Ffmpeg;

public interface IFfmpegService : IScopedDependency
{
    Task<byte[]> CombineMp4VideosAsync(List<string> videoUrls, CancellationToken cancellationToken = default);
}

public class FfmpegService : IFfmpegService
{
     public async Task<byte[]> CombineMp4VideosAsync(List<string> videoDataList, CancellationToken cancellationToken = default)
    {
        var outputFileName = $"{Guid.NewGuid()}.mp4";
        var inputFiles = "";
        var downloadedVideoFiles = new List<string>();
        
        try
        {
            foreach (var videoData in videoDataList)
            {
                /*var videoFileName = $"{Guid.NewGuid()}.mp4";
                await File.WriteAllBytesAsync(videoFileName, videoData, cancellationToken).ConfigureAwait(false);
                downloadedVideoFiles.Add(videoFileName);
                inputFiles += $"-i \"{videoFileName}\" ";*/
                downloadedVideoFiles.Add(videoData);
                inputFiles += $"-i \"{videoData}\" ";
            }

            var filterComplex = $"-filter_complex \"";

            for (int i = 0; i < downloadedVideoFiles.Count; i++)
            {
                filterComplex += $"[{i}:v:0][{i}:a:0]";
            }
            
            filterComplex += $"concat=n={downloadedVideoFiles.Count}:v=1:a=1[outv][outa]\"";

            var combineArguments = $"{inputFiles} {filterComplex} -map \"[outv]\" -map \"[outa]\" {outputFileName}";
            
            Log.Information("Combine command arguments: {combineArguments}", combineArguments);
            
            using (var proc = new Process())
            {
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    RedirectStandardError = true,                               
                    RedirectStandardOutput = true,                               
                    Arguments = combineArguments                               
                };                               
                                           
                proc.OutputDataReceived += (_, e) =>                               
                {                               
                    if (!string.IsNullOrEmpty(e.Data))                               
                    {                               
                        Log.Information("FFmpeg Output: {Output}", e.Data);                     
                    }                               
                };                               
                                               
                proc.Start();
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();

                await proc.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            }
            
            if (File.Exists(outputFileName))
            {
                var resultBytes = await File.ReadAllBytesAsync(outputFileName, cancellationToken).ConfigureAwait(false);

                /*foreach (var fileName in downloadedVideoFiles)
                {
                    File.Delete(fileName);
                }*/
                
                File.Delete(outputFileName);

                return resultBytes;
            }

            Log.Error("Failed to generate the combined video file.");
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while combining MP4 videos.");
            return Array.Empty<byte>();
        }
    }
    
    /*public async Task<byte[]> CombineMp4VideosAsync(List<byte[]> videoDataList, CancellationToken cancellationToken = default)
    {
        var outputFileName = $"{Guid.NewGuid()}.mp4";
        var inputFiles = "";
        var downloadedVideoFiles = new List<string>();
        
        try
        {
            foreach (var videoData in videoDataList)
            {
                var videoFileName = $"{Guid.NewGuid()}.mp4";
                await File.WriteAllBytesAsync(videoFileName, videoData, cancellationToken).ConfigureAwait(false);
                downloadedVideoFiles.Add(videoFileName);
                inputFiles += $"-i \"{videoFileName}\" ";
            }

            var filterComplex = $"-filter_complex \"";

            for (int i = 0; i < downloadedVideoFiles.Count; i++)
            {
                filterComplex += $"[{i}:v:0][{i}:a:0]";
            }
            
            filterComplex += $"concat=n={downloadedVideoFiles.Count}:v=1:a=1[outv][outa]\"";

            var combineArguments = $"{inputFiles} {filterComplex} -map \"[outv]\" -map \"[outa]\" {outputFileName}";
            
            Log.Information("Combine command arguments: {combineArguments}", combineArguments);
            
            using (var proc = new Process())
            {
                proc.StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    RedirectStandardError = true,                               
                    RedirectStandardOutput = true,                               
                    Arguments = combineArguments                               
                };                               
                                           
                proc.OutputDataReceived += (_, e) =>                               
                {                               
                    if (!string.IsNullOrEmpty(e.Data))                               
                    {                               
                        Log.Information("FFmpeg Output: {Output}", e.Data);                     
                    }                               
                };                               
                                               
                proc.Start();
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();

                await proc.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            }
            
            if (File.Exists(outputFileName))
            {
                var resultBytes = await File.ReadAllBytesAsync(outputFileName, cancellationToken).ConfigureAwait(false);

                foreach (var fileName in downloadedVideoFiles)
                {
                    File.Delete(fileName);
                }
                
                File.Delete(outputFileName);

                return resultBytes;
            }

            Log.Error("Failed to generate the combined video file.");
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while combining MP4 videos.");
            return Array.Empty<byte>();
        }
    }*/
}
