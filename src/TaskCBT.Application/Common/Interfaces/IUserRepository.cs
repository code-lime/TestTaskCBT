using TaskCBT.Domain.Entities;

namespace TaskCBT.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<bool> CreateUserByCurrentAsync(string firstName, string? lastName, CancellationToken cancellationToken);
}
