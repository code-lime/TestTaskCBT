using TaskCBT.Application.Common.Models;

namespace TaskCBT.Application.Common.Interfaces;

public interface IEventRepository
{
    Task<int?> CreateEventByCurrentAsync(EventData eventData, CancellationToken cancellationToken);
    Task<EventData?> GetEventByIdAsync(int eventId, CancellationToken cancellationToken);
    Task<IEnumerable<EventData>> GetOwnerEventsByCurrentAsync(CancellationToken cancellationToken);
    Task<IEnumerable<EventData>> GetSubscriptionEventsByCurrentAsync(CancellationToken cancellationToken);
    Task<IEnumerable<EventData>> GetOwnerEventsByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<IEnumerable<EventData>> GetSubscriptionEventsByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<IEnumerable<EventData>> GetAllEventsAsync(CancellationToken cancellationToken);
    Task<bool> ModifyEventByCurrentAsync(EventData eventData, CancellationToken cancellationToken);
    Task<bool> JoinEventByIdByCurrentAsync(int eventId, bool join, CancellationToken cancellationToken);
    Task<IEnumerable<UserData>> GetEventSubscribersByIdAsync(int eventId, CancellationToken cancellationToken);
    Task<UserData?> GetEventOwnerById(int eventId, CancellationToken cancellationToken);
}