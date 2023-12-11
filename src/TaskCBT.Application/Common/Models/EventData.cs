using System.Collections.Immutable;

namespace TaskCBT.Application.Common.Models;

public class EventData
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Type { get; set; }
    public required DateTime Time { get; set; }
    public required int? SubscribersLimit { get; set; }

    public required ImmutableDictionary<string, string> Fields { get; set; }
}
