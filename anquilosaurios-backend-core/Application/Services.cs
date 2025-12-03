using aquilosaurios_backend_core.API;
using aquilosaurios_backend_core.Domain.Models;
using aquilosaurios_backend_core.Infrastructure.External;
using aquilosaurios_backend_core.Infrastructure.Persistence;
using aquilosaurios_backend_core.Shared;

namespace aquilosaurios_backend_core.Application
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync(UserFiltersDTO filters);
        Task CreateUserAsync(UserRegisterDTO dto);
        Task UpdateUserAsync(Guid userId, UserUpdateDTO dto);
        Task AddAchievementsAsync(Guid userId, IEnumerable<AchievementType> achievements);
        Task UpdateUserAccountStatusAsync(Guid userId, bool status);
        Task ChangeAdminPrivilegesAsync(Guid userId, bool adminPrivilege);
        Task VerifyEmailAsync(Guid userId);
    }

    public interface IAuthService
    {
        Task<(User? user, string? token)> AuthenticateAsync(LoginDTO dto);
        Task SignOutAsync(Guid userId);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<IEnumerable<User>> GetUsersAsync(UserFiltersDTO filters)
            => _userRepository.GetByFiltersAsync(filters);

        public async Task CreateUserAsync(UserRegisterDTO dto)
        {
            var user = new User(dto.Name, dto.Email, dto.Username, dto.RawPassword);
            await _userRepository.CreateAsync(user);
        }

        public async Task UpdateUserAsync(Guid userId, UserUpdateDTO dto)
        {
            var user = await _userRepository.GetByIdentifierAsync(userId.ToString());

            if (user == null)
                throw new AnquilosauriosException("User not found");

            if (!string.IsNullOrEmpty(dto.Name))
                user.SetName(dto.Name);

            if (!string.IsNullOrEmpty(dto.RawPassword))
                user.SetPassword(dto.RawPassword);

            await _userRepository.UpdateAsync(user);
        }

        public async Task AddAchievementsAsync(Guid userId, IEnumerable<AchievementType> achievements)
        {
            var user = await _userRepository.GetByIdentifierAsync(userId.ToString());

            if (user == null)
                throw new AnquilosauriosException("User not found");

            foreach (var achievementType in achievements)
            {
                var achievement = new Achievement(
                    achievementType.ToString(),
                    $"Achievement: {achievementType}",
                    DateTime.UtcNow
                );
                user.AddAchievement(achievement);
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateUserAccountStatusAsync(Guid userId, bool status)
        {
            var user = await _userRepository.GetByIdentifierAsync(userId.ToString());

            if (user == null)
                throw new AnquilosauriosException("User not found");

            user.SetIsAccountActive(status);
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangeAdminPrivilegesAsync(Guid userId, bool adminPrivilege)
        {
            var user = await _userRepository.GetByIdentifierAsync(userId.ToString());

            if (user == null)
                throw new AnquilosauriosException("User not found");

            user.SetIsAdmin(adminPrivilege);
            await _userRepository.UpdateAsync(user);
        }

        public async Task VerifyEmailAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdentifierAsync(userId.ToString());

            if (user == null)
                throw new AnquilosauriosException("User not found");

            user.VerifyEmail();
            await _userRepository.UpdateAsync(user);
        }
    }

    public class AuthService : IAuthService
    {
        private readonly IAuthProvider _authProvider;
        private readonly IJwtService _jwtService;

        public AuthService(IAuthProvider authProvider, IJwtService jwtService)
        {
            _authProvider = authProvider;
            _jwtService = jwtService;
        }

        public async Task<(User? user, string? token)> AuthenticateAsync(LoginDTO dto)
        {
            var user = await _authProvider.AuthenticateAsync(dto.Identifier, dto.RawPassword);

            if (user == null)
                return (null, null);

            var token = _jwtService.GenerateToken(user);

            return (user, token);
        }

        public Task SignOutAsync(Guid userId)
        {
            return Task.CompletedTask;
        }
    }
}
