using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception at {Path} {Method}: {Message}", context.Request.Path, context.Request.Method, ex.Message);

        context.Response.ContentType = "application/json";

        HttpStatusCode code = HttpStatusCode.InternalServerError;
        object response;

        if (ex is ArgumentException)
        {
            code = HttpStatusCode.BadRequest;
            _logger.LogWarning("ArgumentException: {Message}", ex.Message);
            response = HttpResponseFactory.CreateFailureResponse<object>(
                context, 
                HttpResponseState.BadRequest,
                false,
                "Bad request parameters",
                ErrorCodes.Validation.BadRequest
                );
        }
        else if (ex is UnauthorizedAccessException)
        {
            code = HttpStatusCode.Unauthorized;
            _logger.LogWarning("UnauthorizedAccessException: {Message}", ex.Message);
            response = HttpResponseFactory.CreateFailureResponse<object>(
                context, 
                HttpResponseState.Unauthorized,
                false,
                "Unauthorized access",
                ErrorCodes.Auth.UnauthorizedRole
                );
        }
        else if (ex is KeyNotFoundException)
        {
            code = HttpStatusCode.NotFound;
            _logger.LogWarning("KeyNotFoundException: {Message}", ex.Message);
            response = HttpResponseFactory.CreateFailureResponse<object>(
                context, 
                HttpResponseState.NotFound,
                false,
                "Key not found",
                ErrorCodes.Auth.KeyNotFound
                );
        }
        else if (ex is InvalidOperationException)
        {
            code = HttpStatusCode.Conflict;
            _logger.LogWarning("InvalidOperationException: {Message}", ex.Message);
            response = HttpResponseFactory.CreateFailureResponse<object>(
                context, 
                HttpResponseState.Conflict,
                false,
                "An invalid operation occurred",
                ErrorCodes.General.ConflictError
                );
        }
        else
        {
            response = HttpResponseFactory.CreateFailureResponse<object>(
                context, 
                HttpResponseState.ServerError,
                false,
                "An unexpected error occurred.",
                ErrorCodes.General.ServerError
                );
        }

        context.Response.StatusCode = (int)code;
        await context.Response.WriteAsync(JsonSerializer.Serialize(
            response,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }
        ));
    }
}
