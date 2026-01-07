using System.Net;

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
      if (ex is AppException appEx)
      {
        await HandleAppExceptionAsync(context, appEx);
        return;
      }

      await HandleExceptionAsync(context, ex);
    }
  }

  private async Task HandleAppExceptionAsync(HttpContext context, AppException ex)
  {
    _logger.LogError(ex, "Unhandled exception at {Path} {Method}: {Message}", context.Request.Path, context.Request.Method, ex.Message);

    var (status, state) = ex switch
    {
      UnauthorizedAppException => (HttpStatusCode.Unauthorized, HttpResponseState.Unauthorized),
      InvalidCredentialsAppException => (HttpStatusCode.Unauthorized, HttpResponseState.Unauthorized),
      InvalidNameIdentifierAppException => (HttpStatusCode.Unauthorized, HttpResponseState.Unauthorized),
      TokenExpiredAppException => (HttpStatusCode.Unauthorized, HttpResponseState.Unauthorized),
      TokenTamperedAppException => (HttpStatusCode.Forbidden, HttpResponseState.Forbidden),
      NotFoundAppException => (HttpStatusCode.NotFound, HttpResponseState.NotFound),
      ConflictAppException => (HttpStatusCode.Conflict, HttpResponseState.Conflict),
      BadRequestAppException => (HttpStatusCode.BadRequest, HttpResponseState.BadRequest),
      ForbiddenAppException => (HttpStatusCode.Forbidden, HttpResponseState.Forbidden),
      AccountBlockedAppException => (HttpStatusCode.Forbidden, HttpResponseState.Forbidden),
      MissingRequiredFieldAppException => (HttpStatusCode.BadRequest, HttpResponseState.BadRequest),
      InvalidFormatAppException => (HttpStatusCode.BadRequest, HttpResponseState.BadRequest), // ? Change http 400 -> 422
      ValueOutOfRangeAppException => (HttpStatusCode.BadRequest, HttpResponseState.BadRequest),
      InternalServerAppException => (HttpStatusCode.InternalServerError, HttpResponseState.ServerError),

      _ => (HttpStatusCode.InternalServerError, HttpResponseState.ServerError)
    };

    var response = HttpResponseFactory.CreateFailureResponse<object>(
      context,
      state,
      false,
      null,
      ex.ErrorCode
    );
    
    context.Response.StatusCode = (int)status;
    context.Response.ContentType = "application/json";

    await context.Response.WriteAsJsonAsync(response);
  }

  private async Task HandleExceptionAsync(HttpContext context, Exception ex)
{
    _logger.LogError(ex,
      "Unhandled exception at {Path} {Method}",
      context.Request.Path,
      context.Request.Method
    );

    if (ex is AppException appEx)
    {
      await HandleAppExceptionAsync(context, appEx);
      return;
    }

    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

    var response = HttpResponseFactory.CreateFailureResponse<object>(
      context,
      HttpResponseState.ServerError,
      false,
      null,
      DomainErrorCodes.GeneralCodes.ServerError
    );

    await context.Response.WriteAsJsonAsync(response);
}
}

