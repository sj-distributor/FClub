using FClub.Messages.Dto.Upload;

namespace FClub.Messages.Dto.File;

public class CombineTaskDto
{
    public Guid TaskId { get; set; }

    public List<string> Ulrs { get; set; }

    public S3UploadDto S3UploadDto { get; set; }
    
    public string FilePath { get; set; }
}