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
    private readonly IConfiguration _config;

    public AuthController(AuthService auth, IConfiguration config)
    {
        _auth = auth;
        _config = config;
    }

    private void SetAuthCookie(string token)
    {
        var cookieName = _config["Jwt:CookieName"] ?? "access_token";
        var expiresIn = int.Parse(_config["Jwt:ExpiresInSeconds"] ?? "3600");
        var isProduction = !HttpContext.Request.Host.Host.Contains("localhost", StringComparison.OrdinalIgnoreCase);

        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = SameSiteMode.Lax,
            Path = "/api",
            MaxAge = TimeSpan.FromSeconds(expiresIn)
        };

        Response.Cookies.Append(cookieName, token, options);
    }

    private void ClearAuthCookie()
    {
        var cookieName = _config["Jwt:CookieName"] ?? "access_token";
        Response.Cookies.Delete(cookieName, new CookieOptions { Path = "/api" });
    }

    /// <summary>Register a new user. Sets HttpOnly cookie and returns user (auto-login).</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var (auth, error) = await _auth.RegisterAsync(request.Name, request.Email, request.Password);
        if (error is not null)
            return BadRequest(new { error });

        SetAuthCookie(auth!.AccessToken);
        return Ok(new { user = auth.User });
    }

    /// <summary>Login. Sets HttpOnly cookie and returns user.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var (auth, error) = await _auth.LoginAsync(request.Email, request.Password);
        if (error is not null)
            return Unauthorized(new { error });

        SetAuthCookie(auth!.AccessToken);
        return Ok(new { user = auth.User });
    }

    /// <summary>Logout. Clears the auth cookie. AllowAnonymous so expired tokens can still clear the cookie.</summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    public IActionResult Logout()
    {
        ClearAuthCookie();
        return NoContent();
    }

    /// <summary>Get current user from cookie.</summary>
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
