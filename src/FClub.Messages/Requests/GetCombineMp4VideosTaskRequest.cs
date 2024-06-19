using Mediator.Net.Contracts;
using FClub.Messages.Responses;

namespace FClub.Messages.Requests;

public class GetCombineMp4VideosTaskRequest : IRequest
{
    public Guid TaskId { get; set; }
}

public class GetCombineMp4VideosTaskResponse : FClubResponse<GetCombineMp4VideoTaskDto>
{
}

public class GetCombineMp4VideoTaskDto
{
    public string? Url { get; set; }
}