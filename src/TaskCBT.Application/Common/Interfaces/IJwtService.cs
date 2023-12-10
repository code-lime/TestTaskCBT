using System.Security.Claims;

namespace TaskCBT.Application.Common.Interfaces;

public interface IJwtService
{
    IJwtBuilder GetJwtBuilder();

    int? GetAuthId(ClaimsPrincipal claimsPrincipal);
    int? GetUserId(ClaimsPrincipal claimsPrincipal);
}
