using aquilosaurios_backend_core.Domain.Models;
using aquilosaurios_backend_core.Infrastructure.External;
using aquilosaurios_backend_core.Shared;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Infrastructure.Tests.External
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;
        private readonly string _secretKey = "THIS_IS_A_TEST_SECRET_KEY_123456789";
        private readonly IConfiguration _config;

        public JwtServiceTests()
        {
            var inMemoryConfig = new Dictionary<string, string>
            {
                { "Jwt:Key", _secretKey },
                { "Jwt:Issuer", "test_issuer" },
                { "Jwt:Audience", "test_audience" },
                { "Jwt:ExpirationMinutes", "1" }
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();

            _jwtService = new JwtService(_config);
        }

        private User CreateTestUser()
        {
            return new User("TestUser", "test@mail.com", "testuser", "pass123")
            {
                IsAdmin = true,
                AuthProvider = AuthProvider.LOCAL
            };
        }

        [Fact]
        public void GenerateToken_ShouldReturn_NonEmptyString()
        {
            var user = CreateTestUser();

            var token = _jwtService.GenerateToken(user);

            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GenerateToken_ShouldContainExpectedClaims()
        {
            var user = CreateTestUser();

            var token = _jwtService.GenerateToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Claims.Should().Contain(c => c.Type == "userId" && c.Value == user.Id.ToString());
            jwt.Claims.Should().Contain(c => c.Type == "name" && c.Value == user.Name);
            jwt.Claims.Should().Contain(c => c.Type == "username" && c.Value == user.Username);
            jwt.Claims.Should().Contain(c => c.Type == "email" && c.Value == user.Email);
            jwt.Claims.Should().Contain(c => c.Type == "isAdmin" && c.Value == "true");
            jwt.Claims.Should().Contain(c => c.Type == "authProvider" && c.Value == user.AuthProvider.ToString());
            jwt.Claims.Should().Contain(c => c.Type == "role" || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        }

        [Fact]
        public void ValidateToken_ShouldReturnTrue_ForValidToken()
        {
            var user = CreateTestUser();
            var token = _jwtService.GenerateToken(user);

            var isValid = _jwtService.ValidateToken(token);

            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidateToken_ShouldReturnFalse_ForTamperedToken()
        {
            var user = CreateTestUser();
            var token = _jwtService.GenerateToken(user);

            var parts = token.Split('.');
            var tamperedToken = $"{parts[0]}.{parts[1]}.INVALID_SIGNATURE";

            var isValid = _jwtService.ValidateToken(tamperedToken);

            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateToken_ShouldReturnFalse_ForExpiredToken()
        {
            var expiredConfig = new Dictionary<string, string>
            {
                { "Jwt:Key", _secretKey },
                { "Jwt:Issuer", "test_issuer" },
                { "Jwt:Audience", "test_audience" },
                { "Jwt:ExpirationMinutes", "-1" }
            };

            var expiredJwtService = new JwtService(
                new ConfigurationBuilder().AddInMemoryCollection(expiredConfig).Build()
            );

            var user = CreateTestUser();
            var token = expiredJwtService.GenerateToken(user);

            var isValid = expiredJwtService.ValidateToken(token);

            isValid.Should().BeFalse();
        }

        [Fact]
        public void GenerateToken_ShouldUseConfiguredIssuerAndAudience()
        {
            var user = CreateTestUser();

            var token = _jwtService.GenerateToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            jwt.Issuer.Should().Be("test_issuer");
            jwt.Audiences.Should().Contain("test_audience");
        }

        [Fact]
        public void JwtService_Constructor_ShouldThrowException_WhenJwtKeyIsMissing()
        {
            var inMemoryConfig = new Dictionary<string, string>
            {
                { "Jwt:Issuer", "test_issuer" },
                { "Jwt:Audience", "test_audience" },
                { "Jwt:ExpirationMinutes", "1" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();

            Action act = () => new JwtService(config);
            act.Should().Throw<Exception>().WithMessage("Jwt:Key no configurado en appsettings.json");
        }

        [Fact]
        public void JwtService_Constructor_ShouldUseDefaultIssuer_WhenJwtIssuerIsMissing()
        {
            var inMemoryConfig = new Dictionary<string, string>
            {
                { "Jwt:Key", _secretKey },
                { "Jwt:Audience", "test_audience" },
                { "Jwt:ExpirationMinutes", "1" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();

            var jwtService = new JwtService(config);

            jwtService.Issuer.Should().Be("aquilosaurios");
        }

        [Fact]
        public void JwtService_Constructor_ShouldUseDefaultAudience_WhenJwtAudienceIsMissing()
        {
            var inMemoryConfig = new Dictionary<string, string>
    {
        { "Jwt:Key", _secretKey },
        { "Jwt:Issuer", "test_issuer" },
        { "Jwt:ExpirationMinutes", "1" }
    };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();

            var jwtService = new JwtService(config);

            jwtService.Audience.Should().Be("aquilosaurios_users");
        }

        [Fact]
        public void JwtService_Constructor_ShouldUseDefaultExpiration_WhenJwtExpirationMinutesIsMissing()
        {
            var inMemoryConfig = new Dictionary<string, string>
            {
                { "Jwt:Key", _secretKey },
                { "Jwt:Issuer", "test_issuer" },
                { "Jwt:Audience", "test_audience" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();

            var jwtService = new JwtService(config);

            jwtService.ExpirationMinutes.Should().Be(15);
        }

        [Fact]
        public void JwtService_Constructor_ShouldThrowException_WhenJwtExpirationMinutesIsInvalid()
        {
            var inMemoryConfig = new Dictionary<string, string>
            {
                { "Jwt:Key", _secretKey },
                { "Jwt:Issuer", "test_issuer" },
                { "Jwt:Audience", "test_audience" },
                { "Jwt:ExpirationMinutes", "invalid" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemoryConfig)
                .Build();

            Action act = () => new JwtService(config);

            act.Should().Throw<FormatException>().WithMessage("The given key 'Jwt:ExpirationMinutes' was not a valid integer.*");
        }
    }
}
