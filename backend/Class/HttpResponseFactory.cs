public static class HttpResponseFactory
{
  private static HttpResponseModel<T> CreateResponse<T> (
    HttpContext context,
    HttpResponseState state,
    bool success,
    string? message = null,
    string? errorCode = null,
    T? data = default
    ) => new()
    {
      Success = success,
      State = state,
      Data = data,
      Message = message,
      TraceId = context.TraceIdentifier,
      ErrorCode = errorCode ?? ErrorCodes.General.UnknownError,
      Timestamp = DateTime.UtcNow
    };


  public static HttpResponseModel<T> CreateFailureResponse<T>(
    HttpContext context,
    HttpResponseState state,
    bool success,
    string? message = null,
    string? errorCode = null,
    T? data = default
    ) => CreateResponse<T>(
      context,
      state,
      false,
      message,
      errorCode,
      data
      );
}