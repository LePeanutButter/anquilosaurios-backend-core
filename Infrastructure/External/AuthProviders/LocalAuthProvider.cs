using System.Security.Cryptography;
using System.Text;
using aquilosaurios_backend_core.Domain.Models;
using aquilosaurios_backend_core.Infrastructure.Persistence;

namespace aquilosaurios_backend_core.Infrastructure.External.AuthProviders;

public class LocalAuthProvider : IAuthProvider
{
    public string ProviderName => "LOCAL";
    
    private readonly IUserRepository _userRepository;

    public LocalAuthProvider(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> AuthenticateAsync(string identifier, string rawPassword)
    {
        // Buscar usuario por email o username
        var user = await _userRepository.GetByIdentifierAsync(identifier);
        
        if (user == null) 
            return null;

        // Verificar contraseña usando el mismo método que tu User.cs
        var hashedInput = HashPassword(rawPassword);
        
        if (user.PasswordHash != hashedInput)
            return null;

        // Verificar que esté activo
        if (!user.IsAccountActive)
            return null;

        return user;
    }

    /// <summary>
    /// Mismo algoritmo que usa tu User.cs
    /// </summary>
    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}