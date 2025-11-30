using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using aquilosaurios_backend_core.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace aquilosaurios_backend_core.Infrastructure.External.JwtAuth;

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtService(IConfiguration config)
    {
        _secretKey = config["Jwt:Key"] ?? throw new Exception("Jwt:Key no configurado en appsettings.json");
        _issuer = config["Jwt:Issuer"] ?? "aquilosaurios";
        _audience = config["Jwt:Audience"] ?? "aquilosaurios_users";
        _expirationMinutes = int.Parse(config["Jwt:ExpirationMinutes"] ?? "15");
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("name", user.Name),
            new Claim("username", user.Username ?? ""),
            new Claim("email", user.Email ?? ""),
            new Claim("isAdmin", user.IsAdmin.ToString().ToLower()),
            new Claim("authProvider", user.AuthProvider.ToString()),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };

            new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}