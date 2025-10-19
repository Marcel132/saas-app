public class JwtMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger _logger;

  public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task Invoke(HttpContext context, MiddlewareService _middlewareService)
{
    string token = null;

    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
    {
        var parts = authHeader.ToString().Split(' ');
        if (parts.Length == 2 && parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            token = parts[1];
    }

    if (string.IsNullOrEmpty(token))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var response = HttpResponseFactory.Unauthorized<object>(
            context,
            "Brak tokenu. Nieautoryzowany dostęp.",
            ErrorCodes.Auth.UnauthorizedRole
        );

        await context.Response.WriteAsJsonAsync(response);
        return; // <- kluczowe, żeby nie przechodzić dalej
    }

    try
    {
        var principal = await _middlewareService.IsValidJwtConfiguration(token);
        context.User = principal;
    }
    catch
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var response = HttpResponseFactory.Unauthorized<object>(
            context,
            "Nieprawidłowy lub wygasły token.",
            ErrorCodes.Auth.UnauthorizedRole
        );

        await context.Response.WriteAsJsonAsync(response);
        return;
    }

    await _next(context);
  }
}