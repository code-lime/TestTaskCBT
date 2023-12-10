using MediatR;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Application.Common.Models;

namespace TaskCBT.Application.Data.Queries;

public record GetAuthByEmailQuery(string Email, string Password) : IRequest<AuthData?>;
public record GetAuthByRefreshTokenQuery(string RefreshToken) : IRequest<AuthData?>;

public class GetAuthQueryHandler(IAuthRepository authRepository) :
    IRequestHandler<GetAuthByEmailQuery, AuthData?>,
    IRequestHandler<GetAuthByRefreshTokenQuery, AuthData?>
{
    public async Task<AuthData?> Handle(GetAuthByEmailQuery request, CancellationToken cancellationToken)
        => await authRepository.GetAuthTokensByEmailAsync(request.Email, request.Password, cancellationToken);
    public async Task<AuthData?> Handle(GetAuthByRefreshTokenQuery request, CancellationToken cancellationToken)
        => await authRepository.GetAuthTokensByRefreshAsync(request.RefreshToken, cancellationToken);
}