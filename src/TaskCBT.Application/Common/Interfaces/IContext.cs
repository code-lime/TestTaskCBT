using Microsoft.EntityFrameworkCore;
using TaskCBT.Domain.Entities;

namespace TaskCBT.Application.Common.Interfaces;

public interface IContext
{
    DbSet<Auth> Auths { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Event> Events { get; }
    DbSet<EventSubscriber> EventSubscribers { get; }
    DbSet<User> Users { get; }

    DbContext DbContext { get; }
}
