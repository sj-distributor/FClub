using Mediator.Net.Contracts;
using FClub.Messages.Responses;

namespace FClub.Messages.Requests;

public class GetCombineMp4VideoTaskRequest : IRequest
{
    public Guid TaskId { get; set; }
}

public class GetCombineMp4VideoTaskResponse : FClubResponse<GetCombineMp4VideoTaskDto>
{
}

public class GetCombineMp4VideoTaskDto
{
    public List<string> OriginalFiles { get; set; }

    public string CombineFile { get; set; }
}