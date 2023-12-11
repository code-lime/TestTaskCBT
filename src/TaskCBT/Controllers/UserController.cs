using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskCBT.Application.Common.Models;
using TaskCBT.Application.Data.Queries;
using TaskCBT.Models;

namespace TaskCBT.Controllers;

[Route("users")]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    [Authorize(Policy = "create")]
    [ProducesResponseType(typeof(SuccessResponse<AuthData>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromQuery(Name = "firstName")] string firstName,
        CancellationToken cancellationToken,
        [FromQuery(Name = "lastName")] string? lastName = null)
        => await mediator.Send(new CreateUserByCurrentQuery(firstName, lastName), cancellationToken) is AuthData authData
            ? Ok(new SuccessResponse<AuthData> { Response = authData })
            : Conflict(new ErrorResponse { Error = "user already exist" });

    [HttpGet("{id}")]
    [Authorize(Policy = "user")]
    [ProducesResponseType(typeof(SuccessResponse<UserData>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(
        [FromRoute(Name = "id")] int id,
        CancellationToken cancellationToken)
        => await mediator.Send(new GetUserByIdQuery(id), cancellationToken) is UserData userData
            ? Ok(new SuccessResponse<UserData> { Response = userData })
            : NotFound(new ErrorResponse { Error = "user not found" });

    [HttpGet("{id}/events")]
    [Authorize(Policy = "user")]
    [ProducesResponseType(typeof(SuccessResponse<IEnumerable<EventData>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        [FromRoute(Name = "id")] int id,
        [FromQuery(Name = "type")] UserTypeQuery type,
        CancellationToken cancellationToken)
        => Ok(new SuccessResponse<IEnumerable<EventData>>
        {
            Response = await mediator.Send(new GetEventsByUserIdQuery(id, type), cancellationToken)
        });

    [HttpPost("modify")]
    [Authorize(Policy = "user")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ModifyAsync(
        [FromBody] UserData userData,
        CancellationToken cancellationToken)
        => await mediator.Send(new ModifyUserByCurrentQuery(userData), cancellationToken)
                ? Ok(new SuccessResponse { })
                : Unauthorized(new ErrorResponse { Error = "user not found" });
}
