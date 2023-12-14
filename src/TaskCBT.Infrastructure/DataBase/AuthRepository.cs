using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Application.Common.Models;
using TaskCBT.Domain.Entities;

namespace TaskCBT.Infrastructure.DataBase;

public class AuthRepository(
    ICurrentUser current,
    ISecure secure,
    IContext context,
    IJwtService jwtService) : IAuthRepository
{
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

    public async Task<AuthData?> GetAuthTokensByIdentityAsync(string identity, string password, CancellationToken cancellationToken)
    {
        Auth? auth = await context.Auths
            .FirstOrDefaultAsync(v => v.Identity == identity
            && (v.Status == AuthStatus.Ok || v.Status == AuthStatus.Registry), 
            cancellationToken);

        if (auth is null) return null;
        if (auth.Data != secure.GetSecure(password, auth.Salt)) return null;
        return await CreateAuthDataAsync(auth, cancellationToken);
    }
    public async Task<AuthData?> GetAuthTokensByRefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        using var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken);
        RefreshToken? refresh = await context.RefreshTokens.FindAsync([refreshToken], cancellationToken);
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
}
