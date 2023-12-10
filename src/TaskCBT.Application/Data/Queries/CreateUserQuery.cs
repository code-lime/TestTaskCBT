using MediatR;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Application.Common.Models;

namespace TaskCBT.Application.Data.Queries;

public record CreateUserQuery(string FirstName, string? LastName) : IRequest<AuthData?>;

public class CreateUserQueryHandler(
    IAuthRepository authRepository,
    IUserRepository userRepository) :
    IRequestHandler<CreateUserQuery, AuthData?>
{
    public async Task<AuthData?> Handle(CreateUserQuery request, CancellationToken cancellationToken)
        => await userRepository.CreateUserByCurrentAsync(request.FirstName, request.LastName, cancellationToken)
        ? await authRepository.GetAuthTokensByCurrentAsync(cancellationToken)
        : null;
}