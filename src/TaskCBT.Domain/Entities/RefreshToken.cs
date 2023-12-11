namespace TaskCBT.Domain.Entities;

public class RefreshToken
{
    public string Token { get; set; } = null!;

    public int AuthId { get; set; }
    public virtual Auth Auth { get; set; } = null!;
}
