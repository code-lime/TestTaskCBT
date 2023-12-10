namespace TaskCBT.Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTime Time { get; set; }

    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public Dictionary<string, string> Fields { get; set; } = [];

    public ICollection<EventSubscriber> Subscribers { get; } = [];
}
