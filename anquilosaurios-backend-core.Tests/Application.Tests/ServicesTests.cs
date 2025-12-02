using aquilosaurios_backend_core.API;
using aquilosaurios_backend_core.Application;
using aquilosaurios_backend_core.Domain.Models;
using aquilosaurios_backend_core.Infrastructure.Persistence;
using aquilosaurios_backend_core.Shared;
using FluentAssertions;
using Moq;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Application.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repoMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _repoMock = new Mock<IUserRepository>();
            _service = new UserService(_repoMock.Object);
        }

        private User CreateUser(Guid id)
        {
            return new User("John", "john@mail.com", "johnny", "1234")
            {
                Id = id
            };
        }

        [Fact]
        public async Task GetUsersAsync_ShouldReturn_FilteredUsers()
        {
            var filters = new UserFiltersDTO { Email = "test@mail.com" };
            var expectedUsers = new List<User> { CreateUser(Guid.NewGuid()) };

            _repoMock.Setup(r => r.GetByFiltersAsync(filters))
                     .ReturnsAsync(expectedUsers);

            var result = await _service.GetUsersAsync(filters);

            result.Should().BeEquivalentTo(expectedUsers);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreate_NewUser()
        {
            var dto = new UserRegisterDTO
            {
                Name = "Test",
                Email = "t@test.com",
                Username = "tester",
                RawPassword = "pass123"
            };

            await _service.CreateUserAsync(dto);

            _repoMock.Verify(r =>
                r.CreateAsync(It.Is<User>(u =>
                    u.Name == dto.Name &&
                    u.Email == dto.Email &&
                    u.Username == dto.Username)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdate_User()
        {
            var id = Guid.NewGuid();
            var existing = CreateUser(id);

            var dto = new UserUpdateDTO
            {
                Name = "NewName",
                RawPassword = "NewPass"
            };

            _repoMock.Setup(r => r.GetByIdentifierAsync(id.ToString()))
                    .ReturnsAsync(existing);

            await _service.UpdateUserAsync(id, dto);

            existing.Name.Should().Be("NewName");
            existing.PasswordHash.Should().NotBeNull();

            _repoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_WhenUserNotFound()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.GetByIdentifierAsync(id.ToString()))
                     .ReturnsAsync((User?)null);

            var dto = new UserUpdateDTO();

            await Assert.ThrowsAsync<AnquilosauriosException>(() =>
                _service.UpdateUserAsync(id, dto));
        }

        [Fact]
        public async Task AddAchievementsAsync_ShouldAdd_Achievements()
        {
            var id = Guid.NewGuid();
            var user = CreateUser(id);

            _repoMock.Setup(r => r.GetByIdentifierAsync(id.ToString()))
                     .ReturnsAsync(user);

            var achievements = new[] { AchievementType.KEEP_IT_MOVING, AchievementType.AIRBORNE_ADDICT };

            await _service.AddAchievementsAsync(id, achievements);

            user.Achievements.Should().HaveCount(2);

            _repoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAccountStatusAsync_ShouldSetStatus()
        {
            var id = Guid.NewGuid();
            var user = CreateUser(id);

            _repoMock.Setup(r => r.GetByIdentifierAsync(id.ToString()))
                     .ReturnsAsync(user);

            await _service.UpdateUserAccountStatusAsync(id, false);

            user.IsAccountActive.Should().BeFalse();
            _repoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task ChangeAdminPrivilegesAsync_ShouldSetAdminFlag()
        {
            var id = Guid.NewGuid();
            var user = CreateUser(id);

            _repoMock.Setup(r => r.GetByIdentifierAsync(id.ToString()))
                     .ReturnsAsync(user);

            await _service.ChangeAdminPrivilegesAsync(id, true);

            user.IsAdmin.Should().BeTrue();
            _repoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task VerifyEmailAsync_ShouldMarkEmailAsVerified()
        {
            var id = Guid.NewGuid();
            var user = CreateUser(id);

            _repoMock.Setup(r => r.GetByIdentifierAsync(id.ToString()))
                     .ReturnsAsync(user);

            await _service.VerifyEmailAsync(id);

            user.IsEmailVerified.Should().BeTrue();
            _repoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }
    }
}
