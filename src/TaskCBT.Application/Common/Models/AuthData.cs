namespace TaskCBT.Application.Common.Models;

public class AuthData
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
