namespace TaskCBT.Infrastructure.Common.Configs;

public class PhoneConfig
{
    public const string SectionKey = "Phone";

    public required string TwilioAccountSid { get; set; }
    public required string TwilioAuthToken { get; set; }
    public required string Phone { get; set; }

    public required string ConfirmMessage { get; set; }
}
