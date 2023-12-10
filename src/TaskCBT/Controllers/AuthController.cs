using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskCBT.Application.Common.Models;
using TaskCBT.Application.Data.Queries;
using TaskCBT.Models;

namespace TaskCBT.Controllers;

[Route("auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpGet("refresh")]
    [AllowAnonymous]
    public async Task<Response> RefreshTokenAsync(
        [FromQuery(Name = "refreshToken")] string refreshToken,
        CancellationToken cancellationToken)
        => await mediator.Send(new GetAuthByRefreshTokenQuery(refreshToken), cancellationToken) is AuthData authData
            ? new SuccessResponse<AuthData> { Response = authData }
            : new ErrorResponse { Error = "refresh not found" };

    [HttpGet("email")]
    [AllowAnonymous]
    public async Task<Response> EmailLoginAsync(
        [FromQuery(Name = "email")] string email,
        [FromQuery(Name = "password")] string password,
        CancellationToken cancellationToken)
        => await mediator.Send(new GetAuthByEmailQuery(email, password), cancellationToken) is AuthData authData
            ? new SuccessResponse<AuthData> { Response = authData }
            : new ErrorResponse { Error = "user not found" };
}
