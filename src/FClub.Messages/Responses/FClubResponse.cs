using System.Net;
using Mediator.Net.Contracts;

namespace FClub.Messages.Responses;

public class FClubResponse<T> : FClubResponse
{
    public T Data { get; set; }
}

public class FClubResponse : IResponse
{
    public HttpStatusCode Code { get; set; }

    public string Msg { get; set; }
}