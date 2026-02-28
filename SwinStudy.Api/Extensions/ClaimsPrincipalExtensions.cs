using System.Security.Claims;

namespace SwinStudy.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : null;
    }

    public static string? GetUserIdString(this ClaimsPrincipal user) =>
        GetUserId(user)?.ToString();
}
