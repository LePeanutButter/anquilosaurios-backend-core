using aquilosaurios_backend_core.Domain.Models;

namespace aquilosaurios_backend_core.Infrastructure.External.AuthProviders;

public interface IAuthProvider
{
    Task<User?> AuthenticateAsync(string identifier, string rawPassword);
    string ProviderName { get; }
}