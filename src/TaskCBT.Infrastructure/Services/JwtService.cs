using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Infrastructure.Common.Configs;

namespace TaskCBT.Infrastructure.Services;

public class JwtService(IOptions<JwtConfig> config) : IJwtService
{
    private readonly JwtConfig _config = config.Value;

    public IJwtBuilder GetJwtBuilder()
        => new Builder(_config);

    public int? GetAuthId(ClaimsPrincipal claimsPrincipal)
        => int.TryParse(claimsPrincipal.FindFirst(ClaimTypes.SerialNumber)?.Value, out int id)
        ? id
        : null;
    public int? GetUserId(ClaimsPrincipal claimsPrincipal)
        => int.TryParse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id)
        ? id
        : null;

    private class Builder : IJwtBuilder
    {
        private readonly List<Claim> _claims = [];
        private readonly JwtConfig _config;

        public Builder(JwtConfig config)
            => _config = config;

        public IJwtBuilder AddAuthId(int id)
        {
            _claims.Add(new Claim(ClaimTypes.SerialNumber, id.ToString()));
            return this;
        }
        public IJwtBuilder AddUserId(int id)
        {
            _claims.Add(new Claim(ClaimTypes.NameIdentifier, id.ToString()));
            return this;
        }
        public IJwtBuilder AddUserName(string name)
        {
            _claims.Add(new Claim(ClaimTypes.Name, name));
            return this;
        }

        public IJwtBuilder AddRole(string role)
        {
            _claims.Add(new Claim(ClaimTypes.Role, role));
            return this;
        }

        public string Build(TimeSpan lifeTime)
        {
            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: _claims,
                expires: DateTime.UtcNow.Add(lifeTime),
                signingCredentials: new SigningCredentials(_config.GetSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
