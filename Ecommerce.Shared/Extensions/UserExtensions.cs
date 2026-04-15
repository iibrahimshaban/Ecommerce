using System.Security.Claims;

namespace Ecommerce.Shared.Extensions;
public static class UserExtensions
{
    public static string? GetUserId(this ClaimsPrincipal User)
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    }
}
