using Amazon.S3;
using Amazon.S3.Model;
using FClub.Core.Ioc;

namespace FClub.Core.Services.Aws;

public interface IAwsS3Service : IScopedDependency
{
    Task UploadFileAsync(AmazonS3Client amazonS3, string bucketName, string fileName, byte[] fileContent, CancellationToken cancellationToken);

    Task<string> GeneratePresignedUrlAsync(AmazonS3Client amazonS3, string bucketName, string fileName, double durationInMinutes = 1);
}

public class AwsS3Service : IAwsS3Service
{
    public async Task UploadFileAsync(AmazonS3Client amazonS3, string bucketName,string fileName, byte[] fileContent, CancellationToken cancellationToken)
    {
        var request = new PutObjectRequest
        {
            Key = fileName,
            BucketName = bucketName,
            InputStream = new MemoryStream(fileContent)
        };
        
        await amazonS3.PutObjectAsync(request, cancellationToken);
    }

    public async Task<string> GeneratePresignedUrlAsync(AmazonS3Client amazonS3, string bucketName, string fileName, double durationInMinutes = 1)
    {
        var request = new GetPreSignedUrlRequest
        {
            Key = fileName,
            BucketName = bucketName,
            Expires = DateTime.UtcNow.AddMinutes(durationInMinutes)
        };
        
        return await amazonS3.GetPreSignedURLAsync(request).ConfigureAwait(false);
    }
}