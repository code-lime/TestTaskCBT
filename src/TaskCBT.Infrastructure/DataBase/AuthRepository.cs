using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Application.Common.Models;
using TaskCBT.Domain.Entities;
using TaskCBT.Infrastructure.Common.Configs;

namespace TaskCBT.Infrastructure.DataBase;

public class AuthRepository(
    ICurrentUser current,
    IEmailService emailService,
    ISecure secure,
    IContext context,
    IJwtService jwtService,
    IOptions<EmailConfig> config) : IAuthRepository
{
    private readonly EmailConfig _config = config.Value;
    private static string GenerateToken(int length = 32)
    {
        byte[] bytes = new byte[length];
        using RandomNumberGenerator generator = RandomNumberGenerator.Create();
        generator.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
    private async Task<AuthData> CreateAuthDataAsync(Auth auth, CancellationToken cancellationToken)
    {
        IJwtBuilder builder = jwtService.GetJwtBuilder()
            .AddAuthId(auth.Id);
        builder = auth.User is User user
            ? builder.AddRole("user")
                .AddUserId(user.Id)
                .AddUserName(user.FirstName + (user.LastName is string lastName ? $" {lastName}" : ""))
            : builder.AddRole("create");

        string accessToken = builder.Build(TimeSpan.FromMinutes(5));
        string refreshToken = GenerateToken();

        RefreshToken refresh = new RefreshToken
        {
            Auth = auth,
            Token = refreshToken
        };
        await context.RefreshTokens.AddAsync(refresh, cancellationToken);
        await context.DbContext.SaveChangesAsync(cancellationToken);

        return new AuthData()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
    private string CreateConfirmToken(Auth auth)
    {
        IJwtBuilder builder = jwtService.GetJwtBuilder()
            .AddRole("confirm")
            .AddAuthId(auth.Id);
        return builder.Build(TimeSpan.FromMinutes(5));
    }

    public async Task<AuthData?> GetAuthTokensByEmailAsync(string email, string password, CancellationToken cancellationToken)
    {
        Auth? auth = await context.Auths
            .FirstOrDefaultAsync(v => v.Identity == email
            && v.Type == AuthType.Email
            && (v.Status == AuthStatus.Ok || v.Status == AuthStatus.Registry), 
            cancellationToken);

        if (auth is null) return null;
        if (auth.Data != secure.GetSecure(password, auth.Salt)) return null;
        return await CreateAuthDataAsync(auth, cancellationToken);
    }
    public async Task<AuthData?> GetAuthTokensByRefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        using var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken);
        RefreshToken? refresh = await context.RefreshTokens
            .FirstOrDefaultAsync(v => v.Token == refreshToken, cancellationToken: cancellationToken);
        if (refresh is null) return null;
        AuthData authData = await CreateAuthDataAsync(refresh.Auth, cancellationToken);
        context.RefreshTokens.Remove(refresh);
        await context.DbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return authData;
    }

    public async Task<AuthData?> GetAuthTokensByCurrentAsync(CancellationToken cancellationToken)
        => current.AuthID is int authId
        && await context.Auths.FindAsync([authId], cancellationToken) is Auth auth
        ? await CreateAuthDataAsync(auth, cancellationToken)
        : null;
    
    public async Task<bool> CreateRegistryByEmailAsync(string email, string password, CancellationToken cancellationToken)
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
        await emailService.SendAsync(email, message.Subject, message.BodyWithArgs(bodyArgs), cancellationToken);
        return true;
    }

    public async Task<bool> ConfirmRegistryAsync(bool confirm, CancellationToken cancellationToken)
    {
        if (current.AuthID is not int authId) return false;
        using var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken);
        Auth? auth = await context.Auths
            .Where(v => v.Id == authId && v.Status == AuthStatus.WaitConfirm)
            .FirstOrDefaultAsync(cancellationToken);
        if (auth is null) return false;
        auth.Status = AuthStatus.Registry;
        await context.DbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}
