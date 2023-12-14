using Microsoft.Extensions.Options;
using System.Text.Json;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Infrastructure.Common.Configs;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace TaskCBT.Infrastructure.Services;

public class PhoneService(IOptions<PhoneConfig> config) : IPhoneService
{
    private readonly PhoneConfig _config = config.Value;

    private ITwilioRestClient? _client;
    private ITwilioRestClient Client => _client ??= new TwilioRestClient(_config.TwilioAccountSid, _config.TwilioAuthToken);

    public async Task<string> SendAsync(string phone, string message, CancellationToken cancellationToken)
    {
        MessageResource resource = await MessageResource.CreateAsync(
            body: message,
            from: new PhoneNumber(_config.Phone),
            to: new PhoneNumber(phone),
            client: Client
        );
        return JsonSerializer.SerializeToNode(resource)?.ToJsonString() ?? "NULL";
    }
}
