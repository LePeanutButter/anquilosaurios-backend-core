using aquilosaurios_backend_core.API;
using aquilosaurios_backend_core.Application;
using aquilosaurios_backend_core.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace aquilosaurios_backend_core.API.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;
    private readonly IAuthService authService;

    public UserController(IUserService userService, IAuthService authService)
    {
        this.userService = userService;
        this.authService = authService;
    }

    // POST users/authenticate
    [HttpPost("authenticate")]
    public async Task<ActionResult<ControllerResponse<object>>> Authenticate([FromBody] LoginDTO dto)
    {
        var (user, token) = await authService.AuthenticateAsync(dto);
        
        if (user == null || token == null)
            return Unauthorized(new ControllerResponse<object>("Invalid credentials"));

        var result = new { user, token };
        return Ok(new ControllerResponse<object>(result, "User authenticated"));
    }

    // POST users/signOut?id=xxx
    [HttpPost("signOut")]
    public async Task<ActionResult<ControllerResponse<string>>> SignOut([FromQuery] Guid id)
    {
        await authService.SignOutAsync(id);
        return Ok(new ControllerResponse<string>("User signed out"));
    }

    [HttpGet]
    public async Task<ActionResult<ControllerResponse<IEnumerable<User>>>> GetUsers(
        [FromQuery] string? name = null,
        [FromQuery] string? email = null,
        [FromQuery] string? username = null,
        [FromQuery] bool? activeStatus = null,
        [FromQuery] bool? adminPrivilege = null
    )
    {
        var filters = new UserFiltersDTO(
            Guid.Empty,
            1,
            20,
            DateTime.MinValue,
            DateTime.MaxValue,
            name ?? string.Empty,
            email ?? string.Empty,
            username ?? string.Empty,
            activeStatus ?? true,
            adminPrivilege ?? false
        );

        var users = await userService.GetUsersAsync(filters);

        return Ok(new ControllerResponse<IEnumerable<User>>(users));
    }

    [HttpPost]
    public async Task<ActionResult<ControllerResponse<string>>> CreateUser([FromBody] UserRegisterDTO dto)
    {
        await userService.CreateUserAsync(dto);
        return Ok(new ControllerResponse<string>("User created successfully"));
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult<ControllerResponse<string>>> UpdateUser(Guid userId, [FromBody] UserUpdateDTO dto)
    {
        await userService.UpdateUserAsync(userId, dto);
        return Ok(new ControllerResponse<string>("User updated successfully"));
    }

    [HttpPut("status/{userId}")]
    public async Task<ActionResult<ControllerResponse<string>>> UpdateStatus(Guid userId, [FromBody] bool status)
    {
        await userService.UpdateUserAccountStatusAsync(userId, status);
        return Ok(new ControllerResponse<string>("Status updated"));
    }

    [HttpPut("admin/{userId}")]
    public async Task<ActionResult<ControllerResponse<string>>> ChangeAdminPrivileges(Guid userId, [FromBody] bool admin)
    {
        await userService.ChangeAdminPrivilegesAsync(userId, admin);
        return Ok(new ControllerResponse<string>("Admin role updated"));
    }

    [HttpPut("verifyEmail/{userId}")]
    public async Task<ActionResult<ControllerResponse<string>>> VerifyEmail(Guid userId)
    {
        await userService.VerifyEmailAsync(userId);
        return Ok(new ControllerResponse<string>("Email verified"));
    }
}