namespace TaskCBT.Domain.Entities;

public class EventSubscriber
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public int EventId { get; set; }
    public virtual Event Event { get; set; } = null!;
}
