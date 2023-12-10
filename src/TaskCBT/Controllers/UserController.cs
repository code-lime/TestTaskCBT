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
    [HttpGet("create")]
    [Authorize(Policy = "create")]
    public async Task<Response> CreateAsync(
        [FromQuery(Name = "firstName")] string firstName,
        CancellationToken cancellationToken,
        [FromQuery(Name = "lastName")] string? lastName = null)
        => await mediator.Send(new CreateUserQuery(firstName, lastName), cancellationToken) is AuthData authData
            ? new SuccessResponse<AuthData> { Response = authData }
            : new ErrorResponse { Error = "auth error" };
}
