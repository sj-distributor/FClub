using Mediator.Net;
using FClub.Messages.Commands;
using FClub.Messages.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FClub.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CombineController : ControllerBase
{
    private readonly IMediator _mediator;

    public CombineController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /*[HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CombineMp4VideoResponse))]
    public async Task<IActionResult> CombineMp4VideosAsync([FromBody] CombineMp4VideoCommand command)
    {
        var response = await _mediator.SendAsync<CombineMp4VideoCommand, CombineMp4VideoResponse>(command);

        return Ok(response);
    }

    [Route("task"), HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CombineMp4VideoTaskResponse))]
    public async Task<IActionResult> CombineMp4VideosTaskAsync([FromBody] CombineMp4VideoTaskCommand command)
    {
        var response = await _mediator.SendAsync<CombineMp4VideoTaskCommand, CombineMp4VideoTaskResponse>(command).ConfigureAwait(false);

        return Ok(response);
    }

    [Route("task/{taskId:guid}"), HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCombineMp4VideoTaskResponse))]
    public async Task<IActionResult> GetCombineMp4VideoTaskAsync(Guid taskId)
    {
        var response = 
            await _mediator.RequestAsync<GetCombineMp4VideoTaskRequest, GetCombineMp4VideoTaskResponse>(new GetCombineMp4VideoTaskRequest
        {
            TaskId = taskId
        }).ConfigureAwait(false);

        return Ok(response);
    }*/
}