using TaskCBT.Application.Common.Models;

namespace TaskCBT.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<bool> CreateUserByCurrentAsync(string firstName, string? lastName, CancellationToken cancellationToken);
    Task<UserData?> GetUserInfoByIdAsync(int userId, CancellationToken cancellationToken);
    Task<UserData?> GetUserInfoByCurrentAsync(CancellationToken cancellationToken);
    Task<bool> ModifyUserInfoByCurrentAsync(UserData userData, CancellationToken cancellationToken);
}