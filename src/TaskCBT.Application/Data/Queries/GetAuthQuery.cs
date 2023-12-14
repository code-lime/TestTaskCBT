using MediatR;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Application.Common.Models;

namespace TaskCBT.Application.Data.Queries;

public record GetAuthByIdentityQuery(string Identity, string Password) : IRequest<AuthData?>;
public record GetAuthByRefreshTokenQuery(string RefreshToken) : IRequest<AuthData?>;

public class GetAuthQueryHandler(
    IAuthRepository authRepository) :
    IRequestHandler<GetAuthByIdentityQuery, AuthData?>,
    IRequestHandler<GetAuthByRefreshTokenQuery, AuthData?>
{
    public async Task<AuthData?> Handle(GetAuthByIdentityQuery request, CancellationToken cancellationToken)
        => await authRepository.GetAuthTokensByIdentityAsync(request.Identity, request.Password, cancellationToken);
    public async Task<AuthData?> Handle(GetAuthByRefreshTokenQuery request, CancellationToken cancellationToken)
        => await authRepository.GetAuthTokensByRefreshAsync(request.RefreshToken, cancellationToken);
}