using Mediator.Net.Contracts;
using FClub.Messages.Dto.Upload;
using FClub.Messages.Responses;

namespace FClub.Messages.Commands;

public class CombineMp4VideoCommand : ICommand
{
    /*public string FilePath { get; set; }

    public S3UploadDto S3UploadDto { get; set; }
    
    public List<string> Urls { get; set; }*/
}

public class CombineMp4VideoResponse : FClubResponse<string>
{
}