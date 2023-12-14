namespace TaskCBT.Application.Common.Interfaces;

public interface IEmailRegistry
{
    Task<bool> CreateRegistryAsync(string email, string password, CancellationToken cancellationToken);
    Task<bool> ConfirmRegistryByCurrentAsync(bool confirm, CancellationToken cancellationToken);
}
