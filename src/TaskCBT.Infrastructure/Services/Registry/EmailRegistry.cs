using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Domain.Entities;
using TaskCBT.Infrastructure.Common.Configs;

namespace TaskCBT.Infrastructure.Services.Registry;

public class EmailRegistry(
    ICurrentUser current,
    IContext context,
    ISecure secure,
    IEmailService service,
    IJwtService jwtService,
    IOptions<EmailConfig> config) : IEmailRegistry
{
    private readonly EmailConfig _config = config.Value;

    private string CreateConfirmToken(Auth auth)
    {
        IJwtBuilder builder = jwtService.GetJwtBuilder()
            .AddRole("confirm")
            .AddAuthId(auth.Id);
        return builder.Build(TimeSpan.FromMinutes(5));
    }
    public async Task<bool> CreateRegistryAsync(string email, string password, CancellationToken cancellationToken)
    {
        Auth? auth;
        await using (var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken))
        {
            auth = await context.Auths
                .Where(v => v.Type == AuthType.Email && v.Identity == email)
                .FirstOrDefaultAsync(cancellationToken);
            string salt = Guid.NewGuid().ToString();
            if (auth is null)
            {
                auth = new Auth
                {
                    Identity = email,
                    Type = AuthType.Email,
                    Status = AuthStatus.WaitConfirm,
                    Data = secure.GetSecure(password, salt),
                    Salt = salt
                };
                await context.Auths.AddAsync(auth, cancellationToken);
            }
            else
            {
                if (auth.Status != AuthStatus.WaitConfirm)
                    return false;
                auth.Data = secure.GetSecure(password, salt);
                auth.Salt = salt;
            }
            await context.DbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        Dictionary<string, string> bodyArgs = new Dictionary<string, string>
        {
            ["token"] = CreateConfirmToken(auth)
        };
        EmailMessage message = _config.ConfirmMessage;
        await service.SendAsync(email, message.Subject, message.Body.WithArgs(bodyArgs), cancellationToken);
        return true;
    }
    public async Task<bool> ConfirmRegistryByCurrentAsync(bool confirm, CancellationToken cancellationToken)
    {
        if (current.AuthID is not int authId) return false;
        using var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken);
        Auth? auth = await context.Auths
            .Where(v => v.Id == authId 
            && v.Type == AuthType.Email 
            && v.Status == AuthStatus.WaitConfirm)
            .FirstOrDefaultAsync(cancellationToken);
        if (auth is null) return false;

        if (confirm) auth.Status = AuthStatus.Registry;
        else context.Auths.Remove(auth);

        await context.DbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}
