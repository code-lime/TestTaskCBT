using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskCBT.Application.Data.Queries;
using TaskCBT.Models;

namespace TaskCBT.Controllers;

[Route("register")]
public class RegisterController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("email")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> EmailLoginAsync(
        [FromQuery(Name = "email")] string email,
        [FromQuery(Name = "password")] string password,
        CancellationToken cancellationToken)
        => await mediator.Send(new RegisterByEmailQuery(email, password), cancellationToken)
            ? Ok(new SuccessResponse { })
            : Conflict(new ErrorResponse { Error = "user already exist" });

    [HttpGet("confirm")]
    [Authorize(Policy = "confirm")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmAsync(CancellationToken cancellationToken)
        => await mediator.Send(new ConfirmEmailByCurrentQuery(true), cancellationToken)
            ? Ok(new SuccessResponse { })
            : Unauthorized(new ErrorResponse { Error = "auth not found" });

    [HttpGet("cancel")]
    [Authorize(Policy = "confirm")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelAsync(CancellationToken cancellationToken)
        => await mediator.Send(new ConfirmEmailByCurrentQuery(false), cancellationToken)
            ? Ok(new SuccessResponse { })
            : Unauthorized(new ErrorResponse { Error = "auth not found" });
}
