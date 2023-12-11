namespace TaskCBT.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public int AuthId { get; set; }
    public virtual Auth Auth { get; set; } = null!;
    public string Token { get; set; } = null!;
}
