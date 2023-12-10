namespace TaskCBT.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }

    public Dictionary<string, string> Fields { get; set; } = [];

    public int? AuthId { get; set; }
    public Auth? Auth { get; set; }

    public ICollection<Event> OwnerEvents { get; } = [];
    public ICollection<EventSubscriber> Subscriptions { get; } = [];
}
