using MediatR;
using TaskCBT.Application.Common.Interfaces;

namespace TaskCBT.Application.Data.Queries;

public record RegisterByEmailQuery(string Email, string Password) : IRequest<bool>;
public record ConfirmEmailByCurrentQuery(bool Confirm) : IRequest<bool>;
public record RegisterByPhoneQuery(string Phone, string Password) : IRequest<bool>;
public record ConfirmPhoneByCodeQuery(string Phone, string Password, string Code) : IRequest<bool>;

public class RegisterQueryHandler(
    IEmailRegistry emailRegistry,
    IPhoneRegistry phoneRegistry) :
    IRequestHandler<RegisterByEmailQuery, bool>,
    IRequestHandler<ConfirmEmailByCurrentQuery, bool>,
    IRequestHandler<RegisterByPhoneQuery, bool>,
    IRequestHandler<ConfirmPhoneByCodeQuery, bool>
{
    public async Task<bool> Handle(RegisterByEmailQuery request, CancellationToken cancellationToken)
        => await emailRegistry.CreateRegistryAsync(request.Email, request.Password, cancellationToken);
    public async Task<bool> Handle(ConfirmEmailByCurrentQuery request, CancellationToken cancellationToken)
        => await emailRegistry.ConfirmRegistryByCurrentAsync(request.Confirm, cancellationToken); 
    
    public async Task<bool> Handle(RegisterByPhoneQuery request, CancellationToken cancellationToken)
        => await phoneRegistry.CreateRegistryAsync(request.Phone, request.Password, cancellationToken);

    public async Task<bool> Handle(ConfirmPhoneByCodeQuery request, CancellationToken cancellationToken)
        => await phoneRegistry.ConfirmRegistryAsync(request.Phone, request.Password, request.Code, cancellationToken);
}