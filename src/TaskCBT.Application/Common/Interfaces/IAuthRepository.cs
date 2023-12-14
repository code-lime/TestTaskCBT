using TaskCBT.Application.Common.Models;

namespace TaskCBT.Application.Common.Interfaces;

public interface IAuthRepository
{
    Task<AuthData?> GetAuthTokensByIdentityAsync(string identity, string password, CancellationToken cancellationToken);
    Task<AuthData?> GetAuthTokensByRefreshAsync(string refreshToken, CancellationToken cancellationToken);
    Task<AuthData?> GetAuthTokensByCurrentAsync(CancellationToken cancellationToken);
}