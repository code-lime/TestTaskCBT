using MediatR;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Application.Common.Models;

namespace TaskCBT.Application.Data.Queries;

public record CreateEventByCurrentQuery(EventData EventData) : IRequest<int?>;
public record ModifyEventByCurrentQuery(EventData EventData) : IRequest<bool>;

public record GetEventByIdQuery(int EventId) : IRequest<EventData?>;
public enum UserTypeQuery
{
    Owner,
    Subscriber
}
public record GetEventsByUserIdQuery(int UserId, UserTypeQuery UserType) : IRequest<IEnumerable<EventData>>;
public record GetEventsAllQuery : IRequest<IEnumerable<EventData>>;

public record JoinEventByIdByCurrentQuery(int EventId, bool Join) : IRequest<bool>;
public record GetEventSubscribersByIdQuery(int EventId) : IRequest<IEnumerable<UserData>>;

public record GetEventOwnerByIdQuery(int EventId) : IRequest<UserData?>;

public class EventQueryHandler(
    IEventRepository eventRepository) :
    IRequestHandler<CreateEventByCurrentQuery, int?>,
    IRequestHandler<ModifyEventByCurrentQuery, bool>,
    IRequestHandler<GetEventByIdQuery, EventData?>,
    IRequestHandler<GetEventsByUserIdQuery, IEnumerable<EventData>>,
    IRequestHandler<GetEventsAllQuery, IEnumerable<EventData>>,
    IRequestHandler<JoinEventByIdByCurrentQuery, bool>,
    IRequestHandler<GetEventSubscribersByIdQuery, IEnumerable<UserData>>,
    IRequestHandler<GetEventOwnerByIdQuery, UserData?>
{
    public async Task<int?> Handle(CreateEventByCurrentQuery request, CancellationToken cancellationToken) 
        => await eventRepository.CreateEventByCurrentAsync(request.EventData, cancellationToken);
    public async Task<bool> Handle(ModifyEventByCurrentQuery request, CancellationToken cancellationToken)
        => await eventRepository.ModifyEventByCurrentAsync(request.EventData, cancellationToken);

    public async Task<EventData?> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        => await eventRepository.GetEventByIdAsync(request.EventId, cancellationToken);
    public async Task<IEnumerable<EventData>> Handle(GetEventsByUserIdQuery request, CancellationToken cancellationToken)
        => request.UserType switch
        {
            UserTypeQuery.Owner => request.UserId == 0
            ? await eventRepository.GetOwnerEventsByCurrentAsync(cancellationToken)
            : await eventRepository.GetOwnerEventsByUserIdAsync(request.UserId, cancellationToken),
            UserTypeQuery.Subscriber => request.UserId == 0
            ? await eventRepository.GetSubscriptionEventsByCurrentAsync(cancellationToken)
            : await eventRepository.GetSubscriptionEventsByUserIdAsync(request.UserId, cancellationToken),
            _ => throw new NotSupportedException($"User type '{request.UserType}'"),
        };
    public async Task<IEnumerable<EventData>> Handle(GetEventsAllQuery request, CancellationToken cancellationToken)
        => await eventRepository.GetAllEventsAsync(cancellationToken);
    public async Task<bool> Handle(JoinEventByIdByCurrentQuery request, CancellationToken cancellationToken)
        => await eventRepository.JoinEventByIdByCurrentAsync(request.EventId, request.Join, cancellationToken);
    public async Task<IEnumerable<UserData>> Handle(GetEventSubscribersByIdQuery request, CancellationToken cancellationToken)
        => await eventRepository.GetEventSubscribersByIdAsync(request.EventId, cancellationToken);
    public async Task<UserData?> Handle(GetEventOwnerByIdQuery request, CancellationToken cancellationToken)
        => await eventRepository.GetEventOwnerById(request.EventId, cancellationToken);
}
