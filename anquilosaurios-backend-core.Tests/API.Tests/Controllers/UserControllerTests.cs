using aquilosaurios_backend_core.API;
using aquilosaurios_backend_core.API.Controllers;
using aquilosaurios_backend_core.Application;
using aquilosaurios_backend_core.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace aquilosaurios_backend_core.Tests.API.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _authServiceMock = new Mock<IAuthService>();
            _controller = new UserController(_userServiceMock.Object, _authServiceMock.Object);
        }

        [Fact]
        public async Task Authenticate_ReturnsOk_WhenValidCredentials()
        {
            var dto = new LoginDTO
            {
                Identifier = "test@example.com",
                RawPassword = "Password123!"
            };
            var user = new User("Test User", "test@example.com", "testuser", "Password123!");
            var token = "dummy-token";

            _authServiceMock.Setup(a => a.AuthenticateAsync(dto)).ReturnsAsync((user, token));

            var actionResult = await _controller.Authenticate(dto);

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<object>>(result.Value);
            Assert.Equal("User authenticated", response.Message);
        }

        [Fact]
        public async Task Authenticate_ReturnsUnauthorized_WhenInvalidCredentials()
        {
            var dto = new LoginDTO
            {
                Identifier = "test@example.com",
                RawPassword = "wrongpassword"
            };

            _authServiceMock.Setup(a => a.AuthenticateAsync(dto)).ReturnsAsync((null, null));

            var actionResult = await _controller.Authenticate(dto);

            var result = actionResult.Result as UnauthorizedObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<object>>(result.Value);
            Assert.Equal("Invalid credentials", response.Message);
        }

        [Fact]
        public async Task SignOut_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            _authServiceMock.Setup(a => a.SignOutAsync(userId)).Returns(Task.CompletedTask);

            var actionResult = await _controller.SignOut(userId);

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<string>>(result.Value);
            Assert.Equal("User signed out", response.Message);
        }

        [Fact]
        public async Task GetUsers_ReturnsUsers()
        {
            var users = new List<User> { new User("Test User", "test@example.com", "testuser", "Password123!") };
            _userServiceMock.Setup(u => u.GetUsersAsync(It.IsAny<UserFiltersDTO>())).ReturnsAsync(users);

            var actionResult = await _controller.GetUsers();

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<IEnumerable<User>>>(result.Value);
            Assert.Equal(users, response.Data);
        }

        [Fact]
        public async Task CreateUser_ReturnsOk()
        {
            var dto = new UserRegisterDTO { Email = "test@example.com", RawPassword = "Password123!" };
            _userServiceMock.Setup(u => u.CreateUserAsync(dto)).Returns(Task.CompletedTask);

            var actionResult = await _controller.CreateUser(dto);

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<string>>(result.Value);
            Assert.Equal("User created successfully", response.Message);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            var dto = new UserUpdateDTO { Name = "Updated Name" };
            _userServiceMock.Setup(u => u.UpdateUserAsync(userId, dto)).Returns(Task.CompletedTask);

            var actionResult = await _controller.UpdateUser(userId, dto);

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<string>>(result.Value);
            Assert.Equal("User updated successfully", response.Message);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            bool status = true;
            _userServiceMock.Setup(u => u.UpdateUserAccountStatusAsync(userId, status)).Returns(Task.CompletedTask);

            var actionResult = await _controller.UpdateStatus(userId, status);

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<string>>(result.Value);
            Assert.Equal("Status updated", response.Message);
        }

        [Fact]
        public async Task ChangeAdminPrivileges_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            bool admin = true;
            _userServiceMock.Setup(u => u.ChangeAdminPrivilegesAsync(userId, admin)).Returns(Task.CompletedTask);

            var actionResult = await _controller.ChangeAdminPrivileges(userId, admin);

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<string>>(result.Value);
            Assert.Equal("Admin role updated", response.Message);
        }

        [Fact]
        public async Task VerifyEmail_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(u => u.VerifyEmailAsync(userId)).Returns(Task.CompletedTask);

            var actionResult = await _controller.VerifyEmail(userId);

            var result = actionResult.Result as OkObjectResult;

            Assert.NotNull(result);
            var response = Assert.IsType<ControllerResponse<string>>(result.Value);
            Assert.Equal("Email verified", response.Message);
        }
    }
}
