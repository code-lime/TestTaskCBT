using System.Collections.Immutable;

namespace TaskCBT.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }

    public ImmutableDictionary<string, string> Fields { get; set; } = ImmutableDictionary<string, string>.Empty;

    public int AuthId { get; set; }
    public virtual Auth Auth { get; set; } = null!;

    public virtual ICollection<Event> OwnerEvents { get; } = [];
    public virtual ICollection<EventSubscriber> Subscriptions { get; } = [];
}
