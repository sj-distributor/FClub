using Serilog;
using Amazon.S3;
using FClub.Core.Ioc;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FClub.Core.Settings.Aws;

namespace FClub.Core.Services.Aws;

public interface IAwsS3Service : IScopedDependency
{
    Task<string> GeneratePresignedUrlAsync(string fileName, double durationInMinutes = 1);

    Task UploadFileToS3StreamAsync(string fileName, string uploadFile, CancellationToken cancellationToken);
}

public class AwsS3Service : IAwsS3Service
{
    private readonly AwsS3Settings _awsS3Settings;
    private readonly AmazonS3Client _amazonS3Client;

    public AwsS3Service(AwsS3Settings awsS3Settings, AmazonS3Client amazonS3Client)
    {
        _awsS3Settings = awsS3Settings;
        _amazonS3Client = amazonS3Client;
    }

    public async Task<string> GeneratePresignedUrlAsync(string fileName, double durationInMinutes = 1)
    {
        var request = new GetPreSignedUrlRequest
        {
            Key = fileName,
            BucketName = _awsS3Settings.BucketName,
            Expires = DateTime.UtcNow.AddMinutes(durationInMinutes)
        };
        
        return await _amazonS3Client.GetPreSignedURLAsync(request).ConfigureAwait(false);
    }
    
    public async Task UploadFileToS3StreamAsync(string fileName, string uploadFile, CancellationToken cancellationToken)
    {
        try
        {
            var transferUtility = new TransferUtility(_amazonS3Client);

            await using var fileStream = File.OpenRead(uploadFile);
            
            await transferUtility.UploadAsync(fileStream, _awsS3Settings.BucketName, fileName, cancellationToken);
        }
        catch (AmazonS3Exception ex)
        {
            Log.Error("AWS failed to upload the file: {@Exception}", ex);
            throw;
        }
        catch (Exception ex)
        {
            Log.Error("Failed to upload file: {@Exception}", ex);
            throw;
        }
    }
}