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
    [AllowAnonymous]
    [HttpGet("refresh")]
    [ProducesResponseType(typeof(SuccessResponse<AuthData>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefreshTokenAsync(
        [FromQuery(Name = "refreshToken")] string refreshToken,
        CancellationToken cancellationToken)
        => await mediator.Send(new GetAuthByRefreshTokenQuery(refreshToken), cancellationToken) is AuthData authData
            ? Ok(new SuccessResponse<AuthData> { Response = authData })
            : NotFound(new ErrorResponse { Error = "refresh not found" });

    [AllowAnonymous]
    [HttpGet("email")]
    [ProducesResponseType(typeof(SuccessResponse<AuthData>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EmailLoginAsync(
        [FromQuery(Name = "email")] string email,
        [FromQuery(Name = "password")] string password,
        CancellationToken cancellationToken)
        => await mediator.Send(new GetAuthByEmailQuery(email, password), cancellationToken) is AuthData authData
            ? Ok(new SuccessResponse<AuthData> { Response = authData })
            : NotFound(new ErrorResponse { Error = "user not found" });
}
