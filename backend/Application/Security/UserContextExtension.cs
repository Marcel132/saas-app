using System.Security.Claims;

namespace backend.Application.Security;

public static class UserContextExtension
{

  /// <summary>
  /// Returns the authenticated user's ID.
  /// Throws <see cref="UnauthorizedAccessException"/> if the claim is missing.
  /// </summary>
  /// <param name="user">Authenticated user.</param>
  /// <returns>User identifier.</returns>
  /// <exception cref="UnauthorizedAccessException">
  /// Thrown when the NameIdentifier claim is missing.
  /// </exception>
  public static Guid GetUserId(this ClaimsPrincipal user)
  {
    var userClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
      ?? throw new UnauthorizedAccessException("User ID claim not found.");

    return Guid.Parse(userClaim);
  }

  /// <summary>
  /// Attempts to retrieve the authenticated user's ID.
  /// Returns <c>null</c> when the user is not authenticated or the claim is missing.
  /// </summary>
  /// <param name="user">Authenticated user.</param>
  /// <returns>User identifier or <c>null</c>.</returns>
  public static Guid? TryGetUserId(this ClaimsPrincipal user)
  {
    var userClaim =
        user.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrWhiteSpace(userClaim))
    {
      return null;
    }

    return Guid.Parse(userClaim);
  }

  public static string GetUserIp(this HttpContext httpContext)
  {
    return httpContext.Connection.RemoteIpAddress?.ToString()
      ?? "Unknown IP Address";
  }

  /// <summary>
  /// Returns the client's User-Agent header.
  /// </summary>
  /// <param name="httpContext">Current HTTP context.</param>
  /// <returns>User-Agent value or "Unknown user-agent".</returns>
  // TODO: Implement X-Forwarded-For header parsing to get real client IP address when behind a proxy or load balancer
  public static string GetUserAgent(this HttpContext httpContext)
  {
    return httpContext.Request.Headers["User-Agent"].ToString()
      ?? "Unknown user-agent";
  }
}