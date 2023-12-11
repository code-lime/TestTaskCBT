using System.Collections.Immutable;

namespace TaskCBT.Application.Common.Models;

public class UserData
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string? LastName { get; set; }

    public required ImmutableDictionary<string, string> Fields { get; set; }
}
