using aquilosaurios_backend_core.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace aquilosaurios_backend_core.Infrastructure.External
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
    }


    public class JwtService : IJwtService
    {
        private readonly string _secretKey;

        public string Issuer { get; }
        public string Audience { get; }
        public int ExpirationMinutes { get; }

        public JwtService(IConfiguration config)
        {
            _secretKey = config["Jwt:Key"] ?? throw new Exception("Jwt:Key no configurado en appsettings.json");
            Issuer = config["Jwt:Issuer"] ?? "aquilosaurios";
            Audience = config["Jwt:Audience"] ?? "aquilosaurios_users";

            if (int.TryParse(config["Jwt:ExpirationMinutes"] ?? "15", out int expiration))
            {
                ExpirationMinutes = expiration;
            }
            else
            {
                throw new FormatException($"The given key 'Jwt:ExpirationMinutes' was not a valid integer. Value: {config["Jwt:ExpirationMinutes"]}");
            }
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
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ExpirationMinutes),
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
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidAudience = Audience,
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
}