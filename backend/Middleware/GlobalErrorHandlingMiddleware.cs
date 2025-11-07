using System.Net;
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
        _logger.LogError(ex.Message);
        _logger.LogError(ex, "Unhandled exception at {Path} {Method}: {Message}", context.Request.Path, context.Request.Method, ex.Message);

        context.Response.ContentType = "application/json";

        HttpStatusCode code = HttpStatusCode.InternalServerError;
        object response;

        if (ex is AppException appEx)
        {
            code = (HttpStatusCode)appEx.StatusCode;
            response = HttpResponseFactory.AppException<object>(context, appEx.Message);
        }
        else if (ex is ArgumentException)
        {
            code = HttpStatusCode.BadRequest;
            response = HttpResponseFactory.BadRequest<object>(context, ex.Message);
        }
        else if (ex is UnauthorizedAccessException)
        {
            code = HttpStatusCode.Unauthorized;
            response = HttpResponseFactory.Unauthorized<object>(context, ex.Message);
        }
        else if (ex is KeyNotFoundException)
        {
            code = HttpStatusCode.NotFound;
            response = HttpResponseFactory.NotFound<object>(context, ex.Message);
        }
        else if (ex is InvalidOperationException)
        {
            code = HttpStatusCode.Conflict;
            response = HttpResponseFactory.Conflict<object>(context, ex.Message);
        }
        else
        {
            response = HttpResponseFactory.InternalServerError<object>(context, "An unexpected error occurred.");
        }

        context.Response.StatusCode = (int)code;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

// using System.Net;
// using System.Text.Json;

// public class GlobalErrorHandlingMiddleware
// {
//     private readonly RequestDelegate _next;
//     private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

//     public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
//     {
//         _next = next;
//         _logger = logger;
//     }

//         public async Task InvokeAsync(HttpContext context)
//     {
//         try
//         {
//             await _next(context);
//         }
//         catch (Exception ex)
//         {
//             await HandleExceptionAsync(context, ex);
//         }
//     }

//     private static Task HandleExceptionAsync(HttpContext context, Exception ex)
//     {
//         var code = HttpStatusCode.InternalServerError;
//         var message = "An internal server error occurred.";

//         if (ex is ArgumentException) { code = HttpStatusCode.BadRequest; message = ex.Message; }
//         else if (ex is KeyNotFoundException) { code = HttpStatusCode.NotFound; message = ex.Message; }
//         else if (ex is UnauthorizedAccessException) { code = HttpStatusCode.Unauthorized; message = ex.Message; }
//         else if (ex is InvalidOperationException) { code = HttpStatusCode.Conflict; message = ex.Message; }
//         else if (ex is ForbiddenAppException) { code = HttpStatusCode.Forbidden; message = ex.Message; }
//         else if (ex is NotFoundAppException) { code = HttpStatusCode.NotFound; message = ex.Message; }

//         context.Response.ContentType = "application/json";
//         context.Response.StatusCode = (int)code;

//         var payload = new { success = false, message = message };
//         return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
//     }

//     // public async Task InvokeAsync(HttpContext context)
//     // {
//     //     var originalBody = context.Response.Body;
//     //     using var newBody = new MemoryStream();
//     //     context.Response.Body = newBody;

//     //     try
//     //     {
//     //         await _next(context);
//     //         _logger.LogInformation("Request processed with status code {StatusCode}", context.Response.StatusCode);


//     //         if (!context.Response.HasStarted)
//     //         {
//     //             if (context.Response.StatusCode >= 400)
//     //             {
//     //                 await HandleStatusCodeAsync(context, context.Response.StatusCode);
//     //                 newBody.Seek(0, SeekOrigin.Begin);
//     //                  await newBody.CopyToAsync(originalBody);
//     //             }
//     //             else
//     //             {
//     //                 newBody.Seek(0, SeekOrigin.Begin);
//     //                 await newBody.CopyToAsync(originalBody);
//     //             }
//     //         }
//     //     }
//     //     catch (AppException ex)
//     //     {
//     //         _logger.LogWarning(ex, "AppException: {Message}", ex.Message);
//     //         await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
//     //     }
//     //     catch (Exception ex)
//     //     {
//     //         _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
//     //         await HandleExceptionAsync(context, 500, "Internal Server Error");
//     //     }
//     //     finally
//     //     {
//     //         context.Response.Body = originalBody;
//     //     }
//     // }

//     // private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
//     // {
//     //     if (context.Response.HasStarted)
//     //         return;

//     //     context.Response.Clear();
//     //     context.Response.StatusCode = statusCode;
//     //     context.Response.ContentType = "application/json";

//     //     var response = new
//     //     {
//     //         status = statusCode,
//     //         error = message
//     //     };

//     //     await context.Response.WriteAsync(JsonSerializer.Serialize(response));
//     // }

//     // private static async Task HandleStatusCodeAsync(HttpContext context, int statusCode)
//     // {
//     //     context.Response.ContentType = "application/json";

//     //     string message = statusCode switch
//     //     {
//     //         400 => "Bad Request",
//     //         401 => "Unauthorized",
//     //         403 => "Forbidden",
//     //         404 => "Not Found",
//     //         409 => "Conflict",
//     //         500 => "Internal Server Error",
//     //         _ => "Unexpected error"
//     //     };

//     //     var response = new
//     //     {
//     //         status = statusCode,
//     //         error = message
//     //     };

//     //     var json = JsonSerializer.Serialize(response);
//     //     await context.Response.WriteAsync(json);
//     // }
// }
