namespace TaskCBT.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync(string email, string subject, string body, CancellationToken cancellationToken);
}
