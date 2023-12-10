using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TaskCBT.Infrastructure.Common.Configs;

public class JwtConfig
{
    public const string SectionKey = "JWT";

    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Key { get; set; } = null!;

    public SymmetricSecurityKey GetSecurityKey() => new(Encoding.UTF8.GetBytes(Key));
}