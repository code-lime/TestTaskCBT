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

    [HttpGet("email/confirm")]
    [Authorize(Policy = "confirm")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> EmailConfirmAsync(CancellationToken cancellationToken)
        => await mediator.Send(new ConfirmEmailByCurrentQuery(true), cancellationToken)
            ? Ok(new SuccessResponse { })
            : Unauthorized(new ErrorResponse { Error = "auth not found" });

    [HttpGet("email/cancel")]
    [Authorize(Policy = "confirm")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> EmailCancelAsync(CancellationToken cancellationToken)
        => await mediator.Send(new ConfirmEmailByCurrentQuery(false), cancellationToken)
            ? Ok(new SuccessResponse { })
            : Unauthorized(new ErrorResponse { Error = "auth not found" });

    [AllowAnonymous]
    [HttpGet("phone")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PhoneLoginAsync(
        [FromQuery(Name = "phone")] string phone,
        [FromQuery(Name = "password")] string password,
        CancellationToken cancellationToken)
        => await mediator.Send(new RegisterByPhoneQuery(phone, password), cancellationToken)
            ? Ok(new SuccessResponse { })
            : Conflict(new ErrorResponse { Error = "user already exist" });

    [AllowAnonymous]
    [HttpGet("phone/confirm")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PhoneConfirmAsync(
        [FromQuery(Name = "phone")] string phone,
        [FromQuery(Name = "password")] string password,
        [FromQuery(Name = "code")] string code,
        CancellationToken cancellationToken)
        => await mediator.Send(new ConfirmPhoneByCodeQuery(phone, password, code), cancellationToken)
            ? Ok(new SuccessResponse { })
            : Conflict(new ErrorResponse { Error = "user already exist" });

}
