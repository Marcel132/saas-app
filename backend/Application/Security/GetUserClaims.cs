using System.Security.Claims;

public static class GetUserClaims
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userClaim == null)
            throw new UnauthorizedAccessException("User ID claim not found.");

        return Guid.Parse(userClaim);
    }

    public static string GetUserIp(HttpContext httpContext)
    {
        var deviceIp = httpContext.Connection.RemoteIpAddress?.ToString();
        if(deviceIp == null)
            throw new BadRequestAppException();

        return deviceIp;
    }
}