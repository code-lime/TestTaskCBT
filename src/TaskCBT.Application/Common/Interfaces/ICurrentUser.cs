namespace TaskCBT.Application.Common.Interfaces;

public interface ICurrentUser
{
    int? AuthID { get; }
    int? UserID { get; }
}
