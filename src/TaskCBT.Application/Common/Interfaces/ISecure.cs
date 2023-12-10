namespace TaskCBT.Application.Common.Interfaces;

public interface ISecure
{
    string GetSecure(string value, string salt);
}
