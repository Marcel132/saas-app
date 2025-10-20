public class JwtMiddleware
{
  private readonly RequestDelegate _next;

  public JwtMiddleware(RequestDelegate next )
  {
    _next = next;
  }

  public async Task Invoke(HttpContext context, MiddlewareService _middlewareService)
  {
    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

    if (!string.IsNullOrEmpty(token))
    {
      try
      {
        var principal = await _middlewareService.IsValidJwtConfiguration(token);
        context.User = principal;
      }
      catch
      {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
        {
          error = "Unauthorized",
          message = "Token is missing, expired or invalid.",
          timestamp = DateTime.UtcNow
        }));
        return;
      }
    }

    await _next(context);
  }
}