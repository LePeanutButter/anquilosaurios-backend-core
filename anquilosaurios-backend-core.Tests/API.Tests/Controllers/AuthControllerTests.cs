using aquilosaurios_backend_core.API;
using aquilosaurios_backend_core.API.Controllers;
using aquilosaurios_backend_core.Application;
using aquilosaurios_backend_core.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace aquilosaurios_backend_core.Tests.API.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _userServiceMock = new Mock<IUserService>();

            _controller = new AuthController(_authServiceMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenSuccessful()
        {
            var dto = new UserRegisterDTO
            {
                Email = "test@example.com",
                RawPassword = "Password123!",
                Name = "Test User",
                Username = "testuser"
            };

            var user = new User(dto.Name, dto.Email, dto.Username, dto.RawPassword);
            var token = "dummy-token";

            _userServiceMock.Setup(u => u.CreateUserAsync(dto)).Returns(Task.CompletedTask);
            _authServiceMock.Setup(a => a.AuthenticateAsync(It.IsAny<LoginDTO>()))
                            .ReturnsAsync((user, token));

            var actionResult = await _controller.Register(dto);

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<object>>(result.Value);
            Assert.Equal("Usuario registrado exitosamente", response.Message);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenExceptionThrown()
        {
            var dto = new UserRegisterDTO();
            _userServiceMock.Setup(u => u.CreateUserAsync(dto))
                            .ThrowsAsync(new Exception("Error"));

            var actionResult = await _controller.Register(dto);

            var result = actionResult.Result as BadRequestObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<object>>(result.Value);
            Assert.Equal("Error", response.Message);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsValid()
        {
            var dto = new LoginDTO("test@example.com", "Password123!");
            var user = new User("Test User", "test@example.com", "testuser", "Password123!");
            var token = "dummy-token";

            _authServiceMock.Setup(a => a.AuthenticateAsync(dto)).ReturnsAsync((user, token));

            var actionResult = await _controller.Login(dto);

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<object>>(result.Value);
            Assert.Equal("Login exitoso", response.Message);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsInvalid()
        {
            var dto = new LoginDTO("test@example.com", "wrongpassword");
            _authServiceMock.Setup(a => a.AuthenticateAsync(dto)).ReturnsAsync((null, null));

            var actionResult = await _controller.Login(dto);

            var result = actionResult.Result as UnauthorizedObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<object>>(result.Value);
            Assert.Equal("Credenciales inválidas", response.Message);
        }

        [Fact]
        public async Task Logout_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("userId", userId.ToString())
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claims }
            };

            _authServiceMock.Setup(a => a.SignOutAsync(userId)).Returns(Task.CompletedTask);

            var actionResult = await _controller.Logout();

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<string>>(result.Value);
            Assert.Equal("Sesión cerrada", response.Message);
        }

        [Fact]
        public void GetCurrentUser_ReturnsUserInfo()
        {
            var userId = Guid.NewGuid();
            var claims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("userId", userId.ToString()),
                new Claim("name", "Test User"),
                new Claim("username", "testuser"),
                new Claim("email", "test@example.com"),
                new Claim("isAdmin", "false"),
                new Claim("authProvider", "Local")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claims }
            };

            var actionResult = _controller.GetCurrentUser();

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<object>>(result.Value);
        }
    }
}
