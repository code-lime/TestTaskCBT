namespace TaskCBT.Domain.Entities;

public class Auth
{
    public int Id { get; set; }
    public AuthType Type { get; set; }
    public string Identity { get; set; } = null!;
    public AuthStatus Status { get; set; }
    public string Data { get; set; } = null!;
    public string Salt { get; set; } = null!;

    public User? User { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; } = [];
}
