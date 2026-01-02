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
        ?? "Unknown device IP";
    }

  public static string GetUserAgent(this HttpContext httpContext)
  {
    var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

    return string.IsNullOrWhiteSpace(userAgent) ? "Unknow user-agent" : userAgent;
  }
}