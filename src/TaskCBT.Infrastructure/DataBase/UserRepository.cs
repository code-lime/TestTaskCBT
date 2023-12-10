using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Domain.Entities;

namespace TaskCBT.Infrastructure.DataBase;

public class UserRepository(ICurrentUser current, IContext context) : IUserRepository
{
    public async Task<bool> CreateUserByCurrentAsync(string firstName, string? lastName, CancellationToken cancellationToken)
    {
        if (current.AuthID is not int authId) return false;
        await using var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken);
        Auth? auth = await context.Auths.FindAsync([authId], cancellationToken);
        if (auth?.User is null) return false;
        User user = new User
        {
            FirstName = firstName,
            LastName = lastName
        };
        await context.Users.AddAsync(user, cancellationToken);
        await context.DbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}