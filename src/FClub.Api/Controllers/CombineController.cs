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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CombineMp4VideosResponse))]
    public async Task<IActionResult> CombineMp4VideosAsync([FromBody] CombineMp4VideosCommand command)
    {
        var response = await _mediator.SendAsync<CombineMp4VideosCommand, CombineMp4VideosResponse>(command);

        return Ok(response);
    }

    [Route("task"), HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CombineMp4VideosTaskResponse))]
    public async Task<IActionResult> CombineMp4VideosTaskAsync([FromBody] CombineMp4VideosTaskCommand command)
    {
        var response = await _mediator.SendAsync<CombineMp4VideosTaskCommand, CombineMp4VideosTaskResponse>(command).ConfigureAwait(false);

        return Ok(response);
    }

    [Route("task/{taskId:guid}"), HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCombineMp4VideosTaskResponse))]
    public async Task<IActionResult> GetCombineMp4VideosTaskAsync(Guid taskId)
    {
        var response = await _mediator.RequestAsync<GetCombineMp4VideosTaskRequest, GetCombineMp4VideosTaskResponse>(
            new GetCombineMp4VideosTaskRequest {TaskId = taskId}).ConfigureAwait(false);

        return Ok(response);
    }
}