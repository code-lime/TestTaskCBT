using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskCBT.Application.Data.Queries;
using TaskCBT.Models;

namespace TaskCBT.Controllers;

[Route("register")]
public class RegisterController(IMediator mediator) : ControllerBase
{
    [HttpGet("email")]
    [AllowAnonymous]
    public async Task<Response> EmailLoginAsync(
        [FromQuery(Name = "email")] string email,
        [FromQuery(Name = "password")] string password,
        CancellationToken cancellationToken)
        => await mediator.Send(new RegisterByEmailQuery(email, password), cancellationToken)
            ? new SuccessResponse { }
            : new ErrorResponse { Error = "user already exist" };

    [HttpGet("confirm")]
    [Authorize(Policy = "confirm")]
    public async Task<Response> ConfirmAsync(CancellationToken cancellationToken)
        => await mediator.Send(new RegisterConfirmQuery(true), cancellationToken)
            ? new SuccessResponse { }
            : new ErrorResponse { Error = "auth not found" };

    [HttpGet("cancel")]
    [Authorize(Policy = "confirm")]
    public async Task<Response> CancelAsync(CancellationToken cancellationToken)
        => await mediator.Send(new RegisterConfirmQuery(false), cancellationToken)
            ? new SuccessResponse { }
            : new ErrorResponse { Error = "auth not found" };
}
