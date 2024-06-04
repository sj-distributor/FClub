using Amazon.S3;
using FClub.Core.Ioc;
using Amazon.S3.Model;
using FClub.Core.Settings.Aws;

namespace FClub.Core.Services.Aws;

public interface IAwsS3Service : IScopedDependency
{
    Task UploadFileAsync(string fileName, byte[] fileContent, CancellationToken cancellationToken);

    Task<string> GeneratePresignedUrlAsync(string fileName, double durationInMinutes = 1);
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

    public async Task UploadFileAsync(string fileName, byte[] fileContent, CancellationToken cancellationToken)
    {
        var request = new PutObjectRequest
        {
            Key = fileName,
            BucketName = _awsS3Settings.BucketName,
            InputStream = new MemoryStream(fileContent)
        };
        
        await _amazonS3Client.PutObjectAsync(request, cancellationToken);
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
}