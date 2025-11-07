public static class HttpResponseFactory
{
  public static HttpResponseModel<T> Ok<T>(HttpContext context, T data, string? message = null, string? errorCode = null)
    => new()
    {
      Success = true,
      State = HttpResponseState.Authorized,
      Data = data,
      Message = message,
      TraceId = context.TraceIdentifier,
      ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
      Timestamp = DateTime.UtcNow
    };
  public static HttpResponseModel<object> Ok(HttpContext context, string? message = null, string? errorCode = null)
    => new()
    {
      Success = true,
      State = HttpResponseState.Authorized,
      Message = message,
      TraceId = context.TraceIdentifier,
      ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
      Timestamp = DateTime.UtcNow
    };

  public static HttpResponseModel<T> Unauthorized<T>(HttpContext context, string? message = null, string? errorCode = null)
    => new()
    {
      Success = false,
      State = HttpResponseState.Unauthorized,
      Message = message,
      TraceId = context.TraceIdentifier,
      ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
      Timestamp = DateTime.UtcNow
    };

  public static HttpResponseModel<T> Forbidden<T>(HttpContext context, string? message = null, string? errorCode = null)
    => new()
    {
      Success = false,
      State = HttpResponseState.Forbidden,
      Message = message,
      TraceId = context.TraceIdentifier,
      ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
      Timestamp = DateTime.UtcNow
    };

  public static HttpResponseModel<T> BadRequest<T>(HttpContext context, string? message = null, string? errorCode = null)
   => new()
   {
     Success = false,
     State = HttpResponseState.BadRequest,
     Message = message,
     TraceId = context.TraceIdentifier,
     ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
      Timestamp = DateTime.UtcNow
   };

  public static HttpResponseModel<T> NotFound<T>(HttpContext context, string? message = null, string? errorCode = null)
   => new()
   {
     Success = false,
     State = HttpResponseState.NotFound,
     Message = message,
     TraceId = context.TraceIdentifier,
     ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
      Timestamp = DateTime.UtcNow
   };

  public static HttpResponseModel<T> InternalServerError<T>(HttpContext context, string? message = null, string? errorCode = null)
   => new()
   {
     Success = false,
     State = HttpResponseState.ServerError,
     Message = message,
     TraceId = context.TraceIdentifier,
     ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
      Timestamp = DateTime.UtcNow
   };

  public static HttpResponseModel<T> Conflict<T>(HttpContext context, string? message = null, string? errorCode = null)
   => new()
   {
     Success = false,
     State = HttpResponseState.Conflict,
     Message = message,
     TraceId = context.TraceIdentifier,
     ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
      Timestamp = DateTime.UtcNow
   };

  public static HttpResponseModel<T> AppException<T>(HttpContext context, string? message = null, string? errorCode = null)
   => new()
   {
     Success = false,
     State = HttpResponseState.ServerError,
     Message = message,
     TraceId = context.TraceIdentifier,
     ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
      Timestamp = DateTime.UtcNow
   }; 
  public static HttpResponseModel<T> FirewallDetected<T>(HttpContext context, string? message = null, string? errorCode = null)
   => new()
   {
     Success = false,
     State = HttpResponseState.FirewallDetected,
     Message = message,
     TraceId = context.TraceIdentifier,
     ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
     Timestamp = DateTime.UtcNow
   };
   
  public static HttpResponseModel<object> FromException(HttpContext context, Exception ex, string? errorCode = null)
    => new()
    {
      Success = false,
      State = HttpResponseState.ServerError,
      Message = ex.Message,
      TraceId = context.TraceIdentifier,
      ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
      Timestamp = DateTime.UtcNow
    };
}