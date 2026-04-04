using System.Security.Claims;

public static class UserContextExtension
{
  public static Guid GetUserId(this ClaimsPrincipal user)
  {
    var userClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
      ?? throw new UnauthorizedAccessException("User ID claim not found.");

    return Guid.Parse(userClaim);
  }

  public static string GetUserIp(this HttpContext httpContext)
  {
    return httpContext.Connection.RemoteIpAddress?.ToString()
      ?? "Unknown IP Address";
  }

  // TODO: Implement X-Forwarded-For header parsing to get real client IP address when behind a proxy or load balancer
  public static string GetUserAgent(this HttpContext httpContext)
  {
    return httpContext.Request.Headers["User-Agent"].ToString()
      ?? "Unknown user-agent";
  }
}