public class AuthenticationMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<AuthenticationMiddleware> _logger;

  public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }


  public async Task InvokeAsync(HttpContext context)
  {
    // Authentication placeholder
    await _next(context);
  }
}