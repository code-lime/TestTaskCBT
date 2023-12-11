using MediatR;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Application.Common.Models;

namespace TaskCBT.Application.Data.Queries;

public record CreateUserByCurrentQuery(string FirstName, string? LastName) : IRequest<AuthData?>;
public record ModifyUserByCurrentQuery(UserData UserData) : IRequest<bool>;
public record GetUserByIdQuery(int UserId) : IRequest<UserData?>;

public class UserQueryHandler(
    IAuthRepository authRepository,
    IUserRepository userRepository) :
    IRequestHandler<CreateUserByCurrentQuery, AuthData?>,
    IRequestHandler<ModifyUserByCurrentQuery, bool>,
    IRequestHandler<GetUserByIdQuery, UserData?>
{
    public async Task<AuthData?> Handle(CreateUserByCurrentQuery request, CancellationToken cancellationToken)
        => await userRepository.CreateUserByCurrentAsync(request.FirstName, request.LastName, cancellationToken)
        ? await authRepository.GetAuthTokensByCurrentAsync(cancellationToken)
        : null;
    public async Task<bool> Handle(ModifyUserByCurrentQuery request, CancellationToken cancellationToken)
        => await userRepository.ModifyUserInfoByCurrentAsync(request.UserData, cancellationToken);
    public async Task<UserData?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        => request.UserId == 0
        ? await userRepository.GetUserInfoByCurrentAsync(cancellationToken)
        : await userRepository.GetUserInfoByIdAsync(request.UserId, cancellationToken);
}