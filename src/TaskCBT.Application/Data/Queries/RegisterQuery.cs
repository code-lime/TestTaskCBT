using MediatR;
using TaskCBT.Application.Common.Interfaces;

namespace TaskCBT.Application.Data.Queries;

public record RegisterByEmailQuery(string Email, string Password) : IRequest<bool>;
public record ConfirmEmailByCurrentQuery(bool Confirm) : IRequest<bool>;

public class RegisterQueryHandler(
    IEmailRegistry emailRegistry) :
    IRequestHandler<RegisterByEmailQuery, bool>,
    IRequestHandler<ConfirmEmailByCurrentQuery, bool>
{
    public async Task<bool> Handle(RegisterByEmailQuery request, CancellationToken cancellationToken)
        => await emailRegistry.CreateRegistryAsync(request.Email, request.Password, cancellationToken);
    public async Task<bool> Handle(ConfirmEmailByCurrentQuery request, CancellationToken cancellationToken)
        => await emailRegistry.ConfirmRegistryByCurrentAsync(request.Confirm, cancellationToken); 
}