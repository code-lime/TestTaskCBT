using System.Security.Claims;
using TaskCBT.Application.Common.Interfaces;

namespace TaskCBT.Services;

public class CurrentUserService : ICurrentUser
{
    private readonly Lazy<int?> _currentAuthID;
    private readonly Lazy<int?> _currentUserID;

    public CurrentUserService(IJwtService jwtService, IHttpContextAccessor contextAccessor)
    {
        _currentAuthID = ReadLazy(contextAccessor, jwtService.GetAuthId);
        _currentUserID = ReadLazy(contextAccessor, jwtService.GetUserId);
    }

    private static Lazy<T?> ReadLazy<T>(IHttpContextAccessor contextAccessor, Func<ClaimsPrincipal, T?> reader) 
        => new Lazy<T?>(() => contextAccessor.HttpContext?.User is ClaimsPrincipal cp ? reader(cp) : default);

    public int? AuthID => _currentAuthID.Value;
    public int? UserID => _currentUserID.Value;
}
