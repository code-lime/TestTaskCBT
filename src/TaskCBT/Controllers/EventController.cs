using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskCBT.Application.Common.Models;
using TaskCBT.Application.Data.Queries;
using TaskCBT.Models;

namespace TaskCBT.Controllers;

[Route("events")]
public class EventController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    [Authorize(Policy = "user")]
    [ProducesResponseType(typeof(SuccessResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] EventData eventData,
        CancellationToken cancellationToken)
        => await mediator.Send(new CreateEventByCurrentQuery(eventData), cancellationToken) is int eventId
            ? Ok(new SuccessResponse<int> { Response = eventId })
            : Unauthorized(new ErrorResponse { Error = "user not found" });

    [HttpGet("{id}")]
    [Authorize(Policy = "user")]
    [ProducesResponseType(typeof(SuccessResponse<EventData>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(
        [FromRoute(Name = "id")] int id,
        CancellationToken cancellationToken)
        => await mediator.Send(new GetEventByIdQuery(id), cancellationToken) is EventData eventData
            ? Ok(new SuccessResponse<EventData> { Response = eventData })
            : NotFound(new ErrorResponse { Error = "event not found" });

    [HttpPost("{id}/join")]
    [Authorize(Policy = "user")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JoinAsync(
        [FromRoute(Name = "id")] int id,
        CancellationToken cancellationToken)
        => await mediator.Send(new JoinEventByIdByCurrentQuery(id, true), cancellationToken)
            ? Ok(new SuccessResponse { })
            : NotFound(new ErrorResponse { Error = "event not found" });

    [HttpPost("{id}/leave")]
    [Authorize(Policy = "user")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LeaveAsync(
        [FromRoute(Name = "id")] int id,
        CancellationToken cancellationToken)
        => await mediator.Send(new JoinEventByIdByCurrentQuery(id, false), cancellationToken)
            ? Ok(new SuccessResponse { })
            : NotFound(new ErrorResponse { Error = "event not found" });

    [HttpGet("{id}/subscribers")]
    [Authorize(Policy = "user")]
    [ProducesResponseType(typeof(SuccessResponse<IEnumerable<UserData>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscribersAsync(
        [FromRoute(Name = "id")] int id,
        CancellationToken cancellationToken)
        => Ok(new SuccessResponse<IEnumerable<UserData>>
        {
            Response = await mediator.Send(new GetEventSubscribersByIdQuery(id), cancellationToken)
        });

    [HttpGet("{id}/owner")]
    [Authorize(Policy = "user")]
    [ProducesResponseType(typeof(SuccessResponse<UserData>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOwnerAsync(
        [FromRoute(Name = "id")] int id,
        CancellationToken cancellationToken)
        => await mediator.Send(new GetEventOwnerByIdQuery(id), cancellationToken) is UserData userData
            ? Ok(new SuccessResponse<UserData> { Response = userData })
            : NotFound(new ErrorResponse { Error = "event not found" });

    [HttpGet]
    [Authorize(Policy = "user")]
    [ProducesResponseType(typeof(SuccessResponse<IEnumerable<EventData>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        CancellationToken cancellationToken)
        => Ok(new SuccessResponse<IEnumerable<EventData>>
        {
            Response = await mediator.Send(new GetEventsAllQuery(), cancellationToken)
        });

    [HttpPost("modify")]
    [Authorize(Policy = "user")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ModifyAsync(
        [FromBody] EventData eventData,
        CancellationToken cancellationToken)
        => await mediator.Send(new ModifyEventByCurrentQuery(eventData), cancellationToken)
            ? Ok(new SuccessResponse { })
            : Unauthorized(new ErrorResponse { Error = "event not found" });
}
