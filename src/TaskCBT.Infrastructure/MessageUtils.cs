using System.Text.RegularExpressions;

namespace TaskCBT.Infrastructure;

public static partial class MessageUtils
{
    [GeneratedRegex(@"\{(\w+)\}", RegexOptions.Compiled)]
    private static partial Regex GetArgMatcherRegex();

    public static string WithArgs(this string text, IDictionary<string, string> args)
        => GetArgMatcherRegex()
        .Replace(text, match => args.TryGetValue(match.Groups[1].Value, out string? value) ? value : match.Value);
}
