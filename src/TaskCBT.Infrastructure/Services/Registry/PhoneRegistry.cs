using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Domain.Entities;
using TaskCBT.Infrastructure.Common.Configs;

namespace TaskCBT.Infrastructure.Services.Registry;

public class PhoneRegistry(
    IContext context,
    ISecure secure,
    IPhoneService service,
    IOptions<PhoneConfig> config) : IPhoneRegistry
{
    private readonly PhoneConfig _config = config.Value;

    private static string CreateConfirmCode() 
        => string.Join("", Enumerable.Range(0, 6).Select(v => (char)(Random.Shared.Next(0, 10) + '0')));

    public async Task<bool> CreateRegistryAsync(string phone, string password, CancellationToken cancellationToken)
    {
        string code = CreateConfirmCode();
        Auth? auth;
        await using (var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken))
        {
            auth = await context.Auths
                .Where(v => v.Type == AuthType.Phone && v.Identity == phone)
                .FirstOrDefaultAsync(cancellationToken);
            string salt = Guid.NewGuid().ToString();
            if (auth is null)
            {
                auth = new Auth
                {
                    Identity = phone,
                    Type = AuthType.Phone,
                    Status = AuthStatus.WaitConfirm,
                    Data = secure.GetSecure(password + code, salt),
                    Salt = salt
                };
                await context.Auths.AddAsync(auth, cancellationToken);
            }
            else
            {
                if (auth.Status != AuthStatus.WaitConfirm)
                    return false;
                auth.Data = secure.GetSecure(password + code, salt);
                auth.Salt = salt;
            }
            await context.DbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        Dictionary<string, string> bodyArgs = new Dictionary<string, string>
        {
            ["code"] = code
        };
        await service.SendAsync(phone, _config.ConfirmMessage.WithArgs(bodyArgs), cancellationToken);
        return true;
    }
    public async Task<bool> ConfirmRegistryAsync(string phone, string password, string code, CancellationToken cancellationToken)
    {
        using var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken);

        Auth? auth = await context.Auths
            .FirstOrDefaultAsync(v => v.Identity == phone 
            && v.Type == AuthType.Phone
            && v.Status == AuthStatus.WaitConfirm,
            cancellationToken);

        if (auth is null) return false;
        if (auth.Data != secure.GetSecure(password + code, auth.Salt)) return false;

        string salt = Guid.NewGuid().ToString();
        auth.Status = AuthStatus.Registry;
        auth.Salt = salt;
        auth.Data = secure.GetSecure(password, salt);

        await context.DbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}
