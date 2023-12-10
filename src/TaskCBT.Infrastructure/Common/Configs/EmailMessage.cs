using System.Text.RegularExpressions;

namespace TaskCBT.Infrastructure.Common.Configs;

public partial class EmailMessage
{
    public required string Subject { get; set; }
    public required string Body { get; set; }

    public string SubjectWithArgs(IDictionary<string, string> args)
        => WithArgs(Subject, args);
    public string BodyWithArgs(IDictionary<string, string> args)
        => WithArgs(Body, args);

    [GeneratedRegex(@"\{(\w+)\}", RegexOptions.Compiled)]
    private static partial Regex GetArgMatcherRegex();

    private static string WithArgs(string text, IDictionary<string, string> args)
        => GetArgMatcherRegex()
        .Replace(text, match => args.TryGetValue(match.Groups[1].Value, out string? value) ? value : match.Value);
}
