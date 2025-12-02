using aquilosaurios_backend_core.Domain.Models;
using aquilosaurios_backend_core.Infrastructure.External;
using aquilosaurios_backend_core.Infrastructure.Persistence;
using aquilosaurios_backend_core.Shared;
using Moq;
using Xunit;
using FluentAssertions;

namespace aquilosaurios_backend_core.Tests.Infrastructure.Tests.External
{
    public class LocalAuthProviderTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly LocalAuthProvider _authProvider;

        public LocalAuthProviderTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _authProvider = new LocalAuthProvider(_mockRepo.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnUser_WhenPasswordMatches()
        {
            var identifier = "test@mail.com";
            var rawPassword = "password123";

            var user = new User("Test", identifier, "testuser", rawPassword);

            _mockRepo
                .Setup(r => r.GetByIdentifierAsync(identifier))
                .ReturnsAsync(user);

            var result = await _authProvider.AuthenticateAsync(identifier, rawPassword);

            result.Should().NotBeNull();
            result!.Email.Should().Be(identifier);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            _mockRepo
                .Setup(r => r.GetByIdentifierAsync("unknown"))
                .ReturnsAsync((User?)null);

            var result = await _authProvider.AuthenticateAsync("unknown", "anything");

            result.Should().BeNull();
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenPasswordDoesNotMatch()
        {
            var identifier = "test@mail.com";

            var user = new User("Test", identifier, "user", "correctPassword");

            _mockRepo
                .Setup(r => r.GetByIdentifierAsync(identifier))
                .ReturnsAsync(user);

            var result = await _authProvider.AuthenticateAsync(identifier, "wrongPassword");

            result.Should().BeNull();
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenAccountIsInactive()
        {
            var identifier = "inactive@mail.com";

            var user = new User("Test", identifier, "user", "password123")
            {
                IsAccountActive = false
            };

            _mockRepo
                .Setup(r => r.GetByIdentifierAsync(identifier))
                .ReturnsAsync(user);

            var result = await _authProvider.AuthenticateAsync(identifier, "password123");

            result.Should().BeNull();
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldCallRepositoryExactlyOnce()
        {
            var identifier = "user@mail.com";
            var password = "pass";

            _mockRepo
                .Setup(r => r.GetByIdentifierAsync(identifier))
                .ReturnsAsync((User?)null);

            await _authProvider.AuthenticateAsync(identifier, password);

            _mockRepo.Verify(r => r.GetByIdentifierAsync(identifier), Times.Once);
        }

        [Fact]
        public void ProviderName_ShouldBeLocal()
        {
            _authProvider.ProviderName.Should().Be(AuthProvider.LOCAL);
        }
    }
}
