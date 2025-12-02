using aquilosaurios_backend_core.Domain.Models;

namespace aquilosaurios_backend_core.Infrastructure.External.JwtAuth;

public interface IJwtService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
}