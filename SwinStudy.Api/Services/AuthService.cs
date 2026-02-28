using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SwinStudy.Api.Data;
using SwinStudy.Api.Dtos;
using SwinStudy.Api.Models;

namespace SwinStudy.Api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<(AuthResponseDto? Auth, string? Error)> RegisterAsync(string email, string password)
    {
        email = email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (null, "Email and password are required.");

        if (password.Length < 8)
            return (null, "Password must be at least 8 characters.");

        var exists = await _db.Users.AnyAsync(u => u.Email == email);
        if (exists)
            return (null, "Email is already registered.");

        var hash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = hash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = GenerateJwt(user);
        var expiresIn = int.Parse(_config["Jwt:ExpiresInSeconds"] ?? "3600");

        return (new AuthResponseDto(token, "Bearer", expiresIn, ToUserDto(user)), null);
    }

    public async Task<(AuthResponseDto? Auth, string? Error)> LoginAsync(string email, string password)
    {
        email = email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return (null, "Email and password are required.");

        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return (null, "Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (null, "Invalid email or password.");

        var token = GenerateJwt(user);
        var expiresIn = int.Parse(_config["Jwt:ExpiresInSeconds"] ?? "3600");

        return (new AuthResponseDto(
            token,
            "Bearer",
            expiresIn,
            ToUserDto(user)), null);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return user is null ? null : ToUserDto(user);
    }

    private string GenerateJwt(User user)
    {
        var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = _config["Jwt:Issuer"] ?? "SwinStudy.Api";
        var audience = _config["Jwt:Audience"] ?? "SwinStudy.Api";
        var expiresIn = int.Parse(_config["Jwt:ExpiresInSeconds"] ?? "3600");

        var keyBytes = Encoding.UTF8.GetBytes(key);
        if (keyBytes.Length < 32)
            throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddSeconds(expiresIn),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserResponseDto ToUserDto(User u) =>
        new UserResponseDto(u.Id, u.Email, u.CreatedAt);
}
