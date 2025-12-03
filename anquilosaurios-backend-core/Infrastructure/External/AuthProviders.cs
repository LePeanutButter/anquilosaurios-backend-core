using aquilosaurios_backend_core.Shared;
using aquilosaurios_backend_core.Domain.Models;
using aquilosaurios_backend_core.Infrastructure.Persistence;
using System.Security.Cryptography;
using System.Text;

namespace aquilosaurios_backend_core.Infrastructure.External
{
    public interface IAuthProvider
    {
        Task<User?> AuthenticateAsync(string identifier, string rawPassword);
        AuthProvider ProviderName { get; }
    }

    public class LocalAuthProvider : IAuthProvider
    {
        public AuthProvider ProviderName => AuthProvider.LOCAL;

        private readonly IUserRepository _userRepository;

        public LocalAuthProvider(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateAsync(string identifier, string rawPassword)
        {
            var user = await _userRepository.GetByIdentifierAsync(identifier);

            if (user == null)
                return null;

            var hashedInput = HashPassword(rawPassword);

            if (user.PasswordHash != hashedInput)
                return null;

            if (!user.IsAccountActive)
                return null;

            return user;
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
