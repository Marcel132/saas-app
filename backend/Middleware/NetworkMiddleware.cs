public class NetworkMiddleware
{
  private readonly ILogger _logger;
  private readonly RequestDelegate _next;

  public NetworkMiddleware(RequestDelegate next, ILogger<NetworkMiddleware> logger)
  {
    _logger = logger;
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {


    // Add to authorization header token from cookie if exists
    if(!context.Request.Headers.ContainsKey("Authorization"))
    {
      var token = context.Request.Cookies["AuthToken"];
      if (!string.IsNullOrEmpty(token))
      {
        context.Request.Headers.Append("Authorization", $"Bearer {token}");
      }
    }

    _logger.LogInformation("Incoming Request: ");
    _logger.LogInformation("Processing request {Method} {Path}", context.Request.Method, context.Request.Path);
    _logger.LogInformation("Request Headers: {Headers}", context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
    _logger.LogInformation("Request Cookies: {Cookies}", context.Request.Cookies.ToDictionary(c => c.Key, c => c.Value));
    _logger.LogInformation("Request QueryString: {QueryString}", context.Request.QueryString.ToString());
    _logger.LogInformation("Request TraceIdentifier: {TraceId}", context.TraceIdentifier);
    _logger.LogInformation("Request RemoteIpAddress: {RemoteIpAddress}", context.Connection.RemoteIpAddress?.ToString());
    _logger.LogInformation("Request LocalIpAddress: {LocalIpAddress}", context.Connection.LocalIpAddress?.ToString());

    await _next(context);
  }
}