using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwinStudy.Api.Dtos;
using SwinStudy.Api.Services;

namespace SwinStudy.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth) => _auth = auth;

    /// <summary>Register a new user. Returns JWT on success (auto-login).</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var (auth, error) = await _auth.RegisterAsync(request.Name, request.Email, request.Password);
        if (error is not null)
            return BadRequest(new { error });

        return Ok(auth);
    }

    /// <summary>Login and receive a JWT access token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var (auth, error) = await _auth.LoginAsync(request.Email, request.Password);
        if (error is not null)
            return Unauthorized(new { error });

        return Ok(auth);
    }

    /// <summary>Get current user from JWT.</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var sub = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            return Unauthorized();

        var user = await _auth.GetUserByIdAsync(userId);
        if (user is null)
            return NotFound();

        return Ok(user);
    }
}
