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
      throw new UnauthorizedAppException("Authorization token is missing.");

    var userClaims = _middlewareService.ValidateToken(token)
    ?? throw new UnauthorizedAppException("Invalid authorization token.");
    
    var identity = new ClaimsIdentity(userClaims, "Custom");
    context.User = new ClaimsPrincipal(identity);

    if (requiredRole != null)
    {
      var role = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
      if (role != requiredRole)
          throw new ForbiddenAppException("User does not have the required role.");
    }

    await _next(context);
  }
}