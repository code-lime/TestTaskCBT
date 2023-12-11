using System.Collections.Immutable;

namespace TaskCBT.Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTime Time { get; set; }

    public int OwnerId { get; set; }
    public virtual User Owner { get; set; } = null!;

    public ImmutableDictionary<string, string> Fields { get; set; } = ImmutableDictionary<string, string>.Empty;

    public virtual ICollection<EventSubscriber> Subscribers { get; } = [];
}
