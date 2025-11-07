using System.Security.Claims;

public class AuthorizationMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<AuthorizationMiddleware> _logger;


  public AuthorizationMiddleware(RequestDelegate next, ILogger<AuthorizationMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var _middlewareService = context.RequestServices.GetRequiredService<MiddlewareService>();

    var endpoint = context.GetEndpoint();
    var requiredRole = endpoint?.Metadata.GetMetadata<RequiredRoleAttribute>()?.Role;
    var allowAnonymous = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null;

    if (allowAnonymous)
    {
      await _next(context);
      return;
    }

    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

    if (string.IsNullOrEmpty(token))
    {
      _logger.LogWarning("Missing authorization token");
      var resp = HttpResponseFactory.Unauthorized<object>(context, "Missing authorization token", ErrorCodes.Auth.InvalidNameIdentifier);
      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      await context.Response.WriteAsJsonAsync(resp);
      return;
    }

    var userClaims = _middlewareService.ValidateToken(token);
    if( userClaims == null)
    {
      _logger.LogWarning("Invalid authorization token");
      var resp = HttpResponseFactory.Unauthorized<object>(context, "Invalid authorization token", ErrorCodes.Auth.InvalidNameIdentifier);
      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      await context.Response.WriteAsJsonAsync(resp);
      return;
    }

    var identity = new ClaimsIdentity(userClaims, "Custom");
    context.User = new ClaimsPrincipal(identity);

    if (requiredRole != null)
    {
      var role = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
      if (role != requiredRole)
      {
        var resp = HttpResponseFactory.Forbidden<object>(context, "You are not allowed to use this method", ErrorCodes.Auth.UnauthorizedRole);
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsJsonAsync(resp);
        _logger.LogWarning("User role '{Role}' does not have access to this resource", role);
        return;
      }
    }

    await _next(context);
  }
}