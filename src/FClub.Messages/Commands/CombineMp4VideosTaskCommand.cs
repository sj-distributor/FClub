using FClub.Messages.Responses;

namespace FClub.Messages.Commands;

public class CombineMp4VideosTaskCommand : CombineMp4VideosCommand
{
    public Guid? TaskId { get; set; }

    public Guid UploadId { get; set; }
}

public class CombineMp4VideosTaskResponse : FClubResponse<Guid>
{
}