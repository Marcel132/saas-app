using System.Text.Json;

public class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

    public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;
        using var newBody = new MemoryStream();
        context.Response.Body = newBody;

        try
        {
            await _next(context);
            _logger.LogInformation("Request processed with status code {StatusCode}", context.Response.StatusCode);

            
            if (!context.Response.HasStarted)
            {
                if (context.Response.StatusCode >= 400)
                {
                    await HandleStatusCodeAsync(context, context.Response.StatusCode);
                }
                else
                {
                    newBody.Seek(0, SeekOrigin.Begin);
                    await newBody.CopyToAsync(originalBody);
                }
            }
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "AppException: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, 500, "Internal Server Error");
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = statusCode,
            error = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static async Task HandleStatusCodeAsync(HttpContext context, int statusCode)
    {
        context.Response.ContentType = "application/json";

        string message = statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            500 => "Internal Server Error",
            _ => "Unexpected error"
        };

        var response = new
        {
            status = statusCode,
            error = message
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
