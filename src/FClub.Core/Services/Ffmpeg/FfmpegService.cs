using Serilog;
using System.IO;
using FClub.Core.Ioc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FClub.Core.Services.Ffmpeg;

public interface IFfmpegService : IScopedDependency
{
    Task<string> CombineMp4VideosAsync(List<string> videoUrls, CancellationToken cancellationToken = default);
}

public class FfmpegService : IFfmpegService
{
    public async Task<string> CombineMp4VideosAsync(List<string> videoUrls, CancellationToken cancellationToken = default)
    {
        var outputFileName = $"{Guid.NewGuid()}.mp4";
        const string inputFileList = "fileList.txt";
        
        try
        {
            Log.Information("Begin reading content to file");
            
            await using (var fileStream = new FileStream(inputFileList, FileMode.Create, FileAccess.Write))
            await using (var writer = new StreamWriter(fileStream))
            {
                foreach (var content in videoUrls)
                {
                    await writer.WriteLineAsync($"file '{content}'");
                }
            }

            var fileLines = await File.ReadAllLinesAsync(inputFileList, cancellationToken).ConfigureAwait(false);
            
            Log.Information("file path: {fileLines}", JsonConvert.SerializeObject(fileLines));
            
            
           var combineArguments = $"-f concat -safe 0 -i \"{inputFileList}\" -c copy \"{outputFileName}\"";
            
           Log.Information("Combine command arguments: {combineArguments}", combineArguments);
           
           using (var proc = new Process())
           {
               proc.StartInfo = new ProcessStartInfo
               {
                   FileName = "ffmpeg",
                   RedirectStandardError = true,
                   RedirectStandardOutput = true,
                   Arguments = combineArguments,
                   UseShellExecute = false,
                   CreateNoWindow = true
               };

               proc.ErrorDataReceived += (_, e) =>
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
               return outputFileName;
           }

           Log.Error("Failed to generate the combined video file.");
           return null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while combining MP4 videos.");
            return null;
        }
        finally
        {
            Log.Information("Combine file finally deleting files");
           
            foreach (var filePath in videoUrls.Where(File.Exists))
            {
                File.Delete(filePath);
            }
            
            if (File.Exists(inputFileList))
                File.Delete(inputFileList);
        }
    }
}
