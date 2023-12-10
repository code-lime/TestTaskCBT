namespace TaskCBT.Infrastructure.Common.Configs;

public class EmailConfig
{
    public const string SectionKey = "Email";

    public string? Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    public required EmailMessage ConfirmMessage { get; set; }
}
