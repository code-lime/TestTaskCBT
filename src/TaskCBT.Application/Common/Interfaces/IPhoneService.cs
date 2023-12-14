namespace TaskCBT.Application.Common.Interfaces;

public interface IPhoneService
{
    Task<string> SendAsync(string phone, string message, CancellationToken cancellationToken);
}
