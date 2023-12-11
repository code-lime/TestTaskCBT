using MediatR;
using TaskCBT.Application.Common.Interfaces;

namespace TaskCBT.Application.Data.Queries;

public record RegisterByEmailQuery(string Email, string Password) : IRequest<bool>;
public record RegisterConfirmQuery(bool Confirm) : IRequest<bool>;

public class RegisterQueryHandler(
    IAuthRepository authRepository) :
    IRequestHandler<RegisterByEmailQuery, bool>,
    IRequestHandler<RegisterConfirmQuery, bool>
{
    public async Task<bool> Handle(RegisterByEmailQuery request, CancellationToken cancellationToken)
        => await authRepository.CreateRegistryByEmailAsync(request.Email, request.Password, cancellationToken);
    public async Task<bool> Handle(RegisterConfirmQuery request, CancellationToken cancellationToken) 
        => await authRepository.ConfirmRegistryAsync(request.Confirm, cancellationToken);
}