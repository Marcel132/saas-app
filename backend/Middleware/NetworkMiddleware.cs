using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

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

    // if (ForbiddenQueryPattern.IsMatch(context.Request.QueryString.Value ?? ""))
    // {
    //   context.Response.StatusCode = StatusCodes.Status400BadRequest;

    //   var response = HttpResponseFactory.CreateFailureResponse<string>(
    //     context,
    //     HttpResponseState.BadRequest,
    //     false,
    //     "Invalid characters detected in query string.",
    //     ErrorCodes.Firewall.QueryStringBlocked
    //   );

    //   context.Response.ContentType = "application/json";
    //   await context.Response.WriteAsJsonAsync(response);
    //   return;
    // }

    // Check for Content-Type header
    if(string.IsNullOrEmpty(context.Request.ContentType))
    {
      context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
      var response = HttpResponseFactory.CreateFailureResponse<string>(
        context,
        HttpResponseState.UnsupportedMediaType,
        false,
        "Unsupported Media Type. Request is missing Content-Type header.",
        HttpStatusCodes.GeneralCodes.UnsupportedMediaType
        );

      context.Response.ContentType = "application/json";
      await context.Response.WriteAsJsonAsync(response);
      return;
    } 
    else if(!context.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
    {
      context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
      var response = HttpResponseFactory.CreateFailureResponse<string>(
        context,
        HttpResponseState.UnsupportedMediaType,
        false,
        "Unsupported Media Type. Request has not proper type.",
        HttpStatusCodes.GeneralCodes.UnsupportedMediaType
        );

      context.Response.ContentType = "application/json";
      await context.Response.WriteAsJsonAsync(response);
      return;
    }

    // Check query string for invalid characters
    
    if (!string.IsNullOrEmpty(context.Request.QueryString.Value))
    {
      var query = context.Request.QueryString.Value;

      if (ForbiddenQueryPattern.IsMatch(query))
      {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var response = HttpResponseFactory.CreateFailureResponse<string>(
          context,
          HttpResponseState.BadRequest,
          false,
          "Invalid characters detected in query string.",
          HttpStatusCodes.FirewallCodes.QueryStringBlocked
        );

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
        return;
      }
    }

    // Check body payload for too large size
    if(context.Request.ContentLength.HasValue && context.Request.ContentLength > 1024 * 1024) // 1 MB limit
      {
        context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
        var response = HttpResponseFactory.CreateFailureResponse<string>(
          context,
          HttpResponseState.ContentTooLarge,
          false,
          "Payload Too Large. Request body exceeds the maximum allowed size.",
          HttpStatusCodes.FirewallCodes.PayloadBlocked
          );

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
        return;
      }
    
    // Handle OPTIONS method for CORS preflight
    foreach(var method in HttpMethodsAllowed.Methods)
    {
      if(context.Request.Method == method)
      {
        break;
      }  
    }
    
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

  private static readonly Regex ForbiddenQueryPattern = new Regex(
    @"(<|>|script|javascript:|'|"")|(\.\./)|(\.\.\\)|(--|/\*|\*/)|\b(union|select|insert|delete|drop|alter)\b",
    RegexOptions.IgnoreCase | RegexOptions.Compiled
);
}