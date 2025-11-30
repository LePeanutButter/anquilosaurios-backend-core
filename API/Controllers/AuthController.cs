using aquilosaurios_backend_core.Application;
using aquilosaurios_backend_core.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aquilosaurios_backend_core.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    /// <summary>
    /// POST api/auth/register
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ControllerResponse<object>>> Register([FromBody] UserRegisterDTO dto)
    {
        try
        {
            await _userService.CreateUserAsync(dto);
            
            // Auto-login después de registro
            var loginDto = new LoginDTO(dto.Email, dto.RawPassword);
            var (user, token) = await _authService.AuthenticateAsync(loginDto);
            
            return Ok(new ControllerResponse<object>(
                new { user, token }, 
                "Usuario registrado exitosamente"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(new ControllerResponse<object>(ex.Message));
        }
    }

    /// <summary>
    /// POST api/auth/login
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ControllerResponse<object>>> Login([FromBody] LoginDTO dto)
    {
        var (user, token) = await _authService.AuthenticateAsync(dto);
        
        if (user == null || token == null)
            return Unauthorized(new ControllerResponse<object>("Credenciales inválidas"));

        return Ok(new ControllerResponse<object>(
            new { user, token }, 
            "Login exitoso"
        ));
    }

    /// <summary>
    /// POST api/auth/logout
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ControllerResponse<string>>> Logout()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        
        if (Guid.TryParse(userIdClaim, out var userId))
            await _authService.SignOutAsync(userId);

        return Ok(new ControllerResponse<string>("Sesión cerrada"));
    }

    /// <summary>
    /// GET api/auth/me - Obtiene info del usuario autenticado
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<ControllerResponse<object>> GetCurrentUser()
    {
        var userInfo = new
        {
            userId = User.FindFirst("userId")?.Value,
            name = User.FindFirst("name")?.Value,
            username = User.FindFirst("username")?.Value,
            email = User.FindFirst("email")?.Value,
            isAdmin = User.FindFirst("isAdmin")?.Value,
            authProvider = User.FindFirst("authProvider")?.Value
        };

        return Ok(new ControllerResponse<object>(userInfo, "Usuario actual"));
    }
}