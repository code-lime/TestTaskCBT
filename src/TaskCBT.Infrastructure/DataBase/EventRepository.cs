using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Application.Common.Models;
using TaskCBT.Domain.Entities;

namespace TaskCBT.Infrastructure.DataBase;

public class EventRepository(ICurrentUser current, IContext context) : IEventRepository
{
    public static EventData MapTo(Event data)
        => new EventData
        {
            Id = data.Id,
            Title = data.Title,
            Type = data.Type,
            Time = data.Time,
            Fields = data.Fields
        };
    public static Event MapTo(EventData data)
        => new Event
        {
            Title = data.Title,
            Type = data.Type,
            Time = data.Time,
            Fields = data.Fields
        };

    public static void SetTo(EventData to, Event from)
    {
        to.Id = from.Id;
        to.Title = from.Title;
        to.Type = from.Type;
        to.Time = from.Time;
        to.Fields = from.Fields;
    }
    public static void SetTo(Event to, EventData from)
    {
        to.Title = from.Title;
        to.Type = from.Type;
        to.Time = from.Time;
        to.Fields = from.Fields;
    }

    public async Task<int?> CreateEventByCurrentAsync(EventData eventData, CancellationToken cancellationToken)
    {
        if (current.UserID is not int userId) return null;
        await using var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken);
        User? user = await context.Users.FindAsync([userId], cancellationToken);
        if (user is null) return null;
        Event _event = MapTo(eventData);
        user.OwnerEvents.Add(_event);
        await transaction.CommitAsync(cancellationToken);
        await context.DbContext.SaveChangesAsync(cancellationToken);
        return _event.Id;
    }

    public async Task<IEnumerable<EventData>> GetAllEventsAsync(CancellationToken cancellationToken)
    {
        List<EventData> events = [];
        await foreach (Event _event in context.Events)
            events.Add(MapTo(_event));
        return events;
    }

    public async Task<EventData?> GetEventByIdAsync(int eventId, CancellationToken cancellationToken) 
        => await context.Events.FindAsync([eventId], cancellationToken) is Event eventData
        ? MapTo(eventData)
        : null;
    
    public async Task<IEnumerable<EventData>> GetOwnerEventsByCurrentAsync(CancellationToken cancellationToken)
        => current.UserID is int userId
        ? await GetOwnerEventsByUserIdAsync(userId, cancellationToken)
        : Enumerable.Empty<EventData>();
    public async Task<IEnumerable<EventData>> GetSubscriptionEventsByCurrentAsync(CancellationToken cancellationToken)
        => current.UserID is int userId
        ? await GetSubscriptionEventsByUserIdAsync(userId, cancellationToken)
        : Enumerable.Empty<EventData>();

    public async Task<IEnumerable<EventData>> GetOwnerEventsByUserIdAsync(int userId, CancellationToken cancellationToken)
        => await context.Users.FindAsync([userId], cancellationToken) is User user
        ? user.OwnerEvents.Select(MapTo)
        : Enumerable.Empty<EventData>();
    public async Task<IEnumerable<EventData>> GetSubscriptionEventsByUserIdAsync(int userId, CancellationToken cancellationToken)
        => await context.Users.FindAsync([userId], cancellationToken) is User user
        ? user.Subscriptions.Select(v => v.Event).Select(MapTo)
        : Enumerable.Empty<EventData>();

    public async Task<bool> ModifyEventByCurrentAsync(EventData eventData, CancellationToken cancellationToken)
    {
        if (current.UserID is not int userId) return false;
        await using var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken);
        Event? _event = await context.Events.FindAsync([eventData.Id], cancellationToken);
        if (_event is null || _event.OwnerId != userId) return false;
        SetTo(_event, eventData);
        await transaction.CommitAsync(cancellationToken);
        await context.DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> JoinEventByIdByCurrentAsync(int eventId, bool join, CancellationToken cancellationToken)
    {
        if (current.UserID is not int userId) return false;
        await using var transaction = await context.DbContext.Database.BeginTransactionAsync(cancellationToken);
        Event? _event = await context.Events.FindAsync([eventId], cancellationToken);
        if (_event is null) return false;
        EventSubscriber? subscriber = _event.Subscribers.FirstOrDefault(v => v.UserId == userId);
        if (subscriber is null && join)
            _event.Subscribers.Add(new EventSubscriber
            {
                Event = _event,
                UserId = userId
            });
        else if (subscriber is not null && !join)
            _event.Subscribers.Remove(subscriber);
        await transaction.CommitAsync(cancellationToken);
        await context.DbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<UserData>> GetEventSubscribersByIdAsync(int eventId, CancellationToken cancellationToken) 
        => await context.Events.FindAsync([eventId], cancellationToken) is Event _event
        ? _event.Subscribers.Select(v => UserRepository.MapTo(v.User))
        : Enumerable.Empty<UserData>();

    public async Task<UserData?> GetEventOwnerById(int eventId, CancellationToken cancellationToken)
        => await context.Events.FindAsync([eventId], cancellationToken) is Event _event
        ? UserRepository.MapTo(_event.Owner)
        : null;
}