using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Application.Common.Models;
using TaskCBT.Domain.Entities;

namespace TaskCBT.Infrastructure.DataBase;

public class UserRepository(ICurrentUser current, IContext context) : IUserRepository
{
    public static UserData MapTo(User data)
        => new UserData
        {
            Id = data.Id,
            FirstName = data.FirstName,
            LastName = data.LastName,
            Fields = data.Fields
        };
    public static User MapTo(UserData data)
        => new User
        {
            FirstName = data.FirstName,
            LastName = data.LastName,
            Fields = data.Fields
        };

    public static void SetTo(UserData to, User from)
    {
        to.Id = from.Id;
        to.FirstName = from.FirstName;
        to.LastName = from.LastName;
        to.Fields = from.Fields;
    }
    public static void SetTo(User to, UserData from)
    {
        to.FirstName = from.FirstName;
        to.LastName = from.LastName;
        to.Fields = from.Fields;
    }

    public async Task<bool> CreateUserByCurrentAsync(string firstName, string? lastName, CancellationToken cancellationToken)
    {
        if (current.AuthID is not int authId) return false;
        await using var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken);
        Auth? auth = await context.Auths.FindAsync([authId], cancellationToken);
        if (auth is null || auth.User is not null) return false;
        User user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Auth = auth
        };
        await context.Users.AddAsync(user, cancellationToken);
        await context.DbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<UserData?> GetUserInfoByIdAsync(int userId, CancellationToken cancellationToken)
    {
        User? user = await context.Users.FindAsync([userId], cancellationToken);
        return user is null ? null : MapTo(user);
    }
    public async Task<UserData?> GetUserInfoByCurrentAsync(CancellationToken cancellationToken)
        => current.UserID is int userId
        ? await GetUserInfoByIdAsync(userId, cancellationToken)
        : null;
    public async Task<bool> ModifyUserInfoByCurrentAsync(UserData userData, CancellationToken cancellationToken)
    {
        if (current.UserID is not int userId) return false;
        await using var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken);
        User? user = await context.Users.FindAsync([userId], cancellationToken);
        if (user is null) return false;
        SetTo(user, userData);
        await context.DbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}