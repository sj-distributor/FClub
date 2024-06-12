using FClub.Messages.Responses;

namespace FClub.Messages.Commands;

public class CombineMp4VideoTaskCommand : CombineMp4VideoCommand
{
    public Guid? TaskId { get; set; }
}

public class CombineMp4VideoTaskResponse : FClubResponse<Guid>
{
}