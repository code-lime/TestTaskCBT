using TaskCBT.Application.Common.Models;

namespace TaskCBT.Application.Common.Interfaces;

public interface IAuthRepository
{
    Task<AuthData?> GetAuthTokensByEmailAsync(string email, string password, CancellationToken cancellationToken);
    Task<AuthData?> GetAuthTokensByRefreshAsync(string refreshToken, CancellationToken cancellationToken);
    Task<AuthData?> GetAuthTokensByCurrentAsync(CancellationToken cancellationToken);
    Task<bool> CreateRegistryByEmailAsync(string email, string password, CancellationToken cancellationToken);
    Task<bool> ConfirmRegistryAsync(bool confirm, CancellationToken cancellationToken);
}