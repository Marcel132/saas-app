using System.Collections.Concurrent;

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
    var currentDateTime = DateTime.UtcNow;
    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    if(HasBody(context))
    {
      if(
        string.IsNullOrEmpty(context.Request.ContentType) ||
        !context.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase)
      )
      {
        context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
        var response = HttpResponseFactory.CreateFailureResponse<string>(
          context,
          HttpResponseState.UnsupportedMediaType,
          false,
          "Unsupported Media Type. Request is missing Content-Type header.",
          DomainErrorCodes.GeneralCodes.UnsupportedMediaType
          );

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
        return;
      }
    
      if(
        context.Request.ContentLength.HasValue
        && context.Request.ContentLength > 1024 * 1024 // 1 MB limit
      )
      {
        context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
        var response = HttpResponseFactory.CreateFailureResponse<string>(
          context,
          HttpResponseState.ContentTooLarge,
          false,
          "Payload Too Large. Request body exceeds the maximum allowed size.",
          DomainErrorCodes.FirewallCodes.PayloadBlocked
          );

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
        return;
      }

    }

  // Rate limiting
  // TODO: Implement a more robust rate limiting strategy, possibly using a distributed cache like Redis for better performance and scalability.
    if(!IsAllowed(ip))
    {
      context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
      var response = HttpResponseFactory.CreateFailureResponse<string>(
        context,
        HttpResponseState.TooManyRequests,
        false,
        "Too Many Requests. You have exceeded the rate limit.",
        DomainErrorCodes.FirewallCodes.RateLimitExceeded
        );

      context.Response.ContentType = "application/json";
      await context.Response.WriteAsJsonAsync(response);
      return;
    }

    // Set headers 
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Content-Security-Policy"] = 
      "default-src 'self';" + 
      "frame-ancestors 'none';" + 
      "object-src 'none';" +
      "base-uri 'self';" +

      "script-src 'self';" +
      "style-src 'self' 'unsafe-inline';" +
      "img-src 'self' data:;" +
      "connect-src 'self';" + 
      "form-action 'self';";

    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=(), payment=()";

    await _next(context);

    _logger.LogInformation(
      "Response {Method} {Path} from {RemoteIpAddress} at {DateTime} // {StatusCode}",
      context.Request.Method,
      context.Request.Path,
      ip,
      currentDateTime,
      context.Response.StatusCode
    );

  }

  private bool HasBody(HttpContext context)
  {
    return HttpMethods.IsPost(context.Request.Method) ||
      HttpMethods.IsPut(context.Request.Method) ||
      HttpMethods.IsPatch(context.Request.Method);
  }
  private static readonly ConcurrentDictionary<string, RateLimitBucket> _buckets = new();
  private bool IsAllowed(string ip)
  {
    var now = DateTime.UtcNow;

    if (!_buckets.TryGetValue(ip, out var bucket))
    {
      bucket = new RateLimitBucket
      {
        Token = 10, // Initial token count
        LastRefill = now
      };
      _buckets[ip] = bucket;
    }

    // Refill tokens based on elapsed time
    var elapsedSeconds = (now - bucket.LastRefill).TotalSeconds;
    var tokensToAdd = (double)(elapsedSeconds * (10.0 / 60)); // Refill rate: 10 tokens per minute
    if (tokensToAdd > 0)
    {
      bucket.Token = Math.Min(bucket.Token + tokensToAdd, 10); // Max tokens: 10
      bucket.LastRefill = now;
    }

    if (bucket.Token >= 1)
    {
      bucket.Token--;
      return true; // Request allowed
    }

    return false; // Request denied
  }
}