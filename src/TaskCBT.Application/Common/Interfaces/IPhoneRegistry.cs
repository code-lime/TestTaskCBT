namespace TaskCBT.Application.Common.Interfaces;

public interface IPhoneRegistry
{
    Task<bool> CreateRegistryAsync(string phone, string password, CancellationToken cancellationToken);
    Task<bool> ConfirmRegistryAsync(string phone, string password, string code, CancellationToken cancellationToken);
}
