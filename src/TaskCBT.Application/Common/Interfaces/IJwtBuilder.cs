namespace TaskCBT.Application.Common.Interfaces;

public interface IJwtBuilder
{
    IJwtBuilder AddAuthId(int id);
    IJwtBuilder AddUserId(int id);
    IJwtBuilder AddUserName(string name);

    IJwtBuilder AddRole(string role);

    string Build(TimeSpan lifeTime);
}