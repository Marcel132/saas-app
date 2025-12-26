using System.Security.Claims;

public static class GetUserClaims
{
public static Guid GetUserId(this ClaimsPrincipal user)
{
    return Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
}