using System.Net;
using Mediator.Net.Contracts;

namespace FClub.Messages.Responses;

public class CommonResponse<T> : CommonResponse
{
    public T Data { get; set; }
}

public class CommonResponse : IResponse
{
    public HttpStatusCode Code { get; set; }

    public string Msg { get; set; }
}