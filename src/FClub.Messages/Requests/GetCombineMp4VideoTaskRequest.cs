using FClub.Messages.Responses;
using Mediator.Net.Contracts;

namespace FClub.Messages.Requests;

public class GetCombineMp4VideoTaskRequest : IRequest
{
    public Guid TaskId { get; set; }
}

public class GetCombineMp4VideoTaskResponse : FClubResponse<List<string>>
{
}