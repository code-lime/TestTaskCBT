using System.Security.Cryptography;
using System.Text;
using TaskCBT.Application.Common.Interfaces;

namespace TaskCBT.Services;

public class SecureSHA256 : ISecure
{
    private readonly string _pepper;
    private readonly int _iterations;

    public SecureSHA256(IConfiguration configuration)
    {
        IConfigurationSection secure = configuration.GetRequiredSection("Secure");
        _pepper = secure.GetValue<string>("Pepper")!;
        _iterations = secure.GetValue<int>("Iterations")!;
    }

    public string GetSecure(string value, string salt)
        => ComputeHash(value, salt, _pepper, _iterations);

    private static string ComputeHash(string password, string salt, string pepper, int iteration)
    {
        if (iteration <= 0) return password;
        var passwordSaltPepper = $"{password}{salt}{pepper}";
        var byteValue = Encoding.UTF8.GetBytes(passwordSaltPepper);
        var byteHash = SHA256.HashData(byteValue);
        var hash = Convert.ToBase64String(byteHash);
        return ComputeHash(hash, salt, pepper, iteration - 1);
    }
}
