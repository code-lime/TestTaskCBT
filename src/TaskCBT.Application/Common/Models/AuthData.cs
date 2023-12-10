using System.Text.Json.Serialization;

namespace TaskCBT.Application.Common.Models;

public class AuthData
{
    public required string AccessToken;
    public required string RefreshToken;
}
