namespace TaskCBT.WebApp;

public class Variable
{
    public const string SectionKey = "Variable";
    public required string UrlApi { get; set; }

    public static Variable Raw { get; internal set; } = null!;
}
