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
      ErrorCode = errorCode ?? HttpStatusCodes.GeneralCodes.ServerError,
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
      success,
      message,
      errorCode,
      data
      );

  public static HttpResponseModel<T> CreateSuccessResponse<T>(
    HttpContext context,
    HttpResponseState state,
    bool success,
    string? message = null,
    string? successCode = null,
    T? data = default
    ) => CreateResponse<T>(
      context,
      state,
      success,
      message,
      successCode,
      data
      );
}