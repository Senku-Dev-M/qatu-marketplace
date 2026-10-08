using System.Security.Claims;

namespace Qatu.Api;

internal static class UserClaims
{
    public static Guid Id(this ClaimsPrincipal user) => Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    public static string Email(this ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.Email) ?? "";
}
