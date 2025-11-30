using aquilosaurios_backend_core.API;
using aquilosaurios_backend_core.Domain.Models;
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
}