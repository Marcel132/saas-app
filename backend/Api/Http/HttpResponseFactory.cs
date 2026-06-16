namespace backend.Api.Http;

public static class HttpResponseFactory
{
  private static HttpResponseModel<T> CreateResponse<T>(
    HttpContext context,
    HttpResponseState state,
    bool success,
    string? message = null,
    string? code = null,
    T? data = default
    ) => new()
    {
      Success = success,
      State = state,
      Data = data,
      Message = message,
      TraceId = context.TraceIdentifier,
      Code = code,
      Timestamp = DateTime.UtcNow
    };

  public static HttpResponseModel<T> CreateFailureResponse<T>(
    HttpContext context,
    HttpResponseState state,
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

  public static HttpResponseModel<T> CreateSuccessResponse<T>(
    HttpContext context,
    HttpResponseState state,
    string? message = null,
    string? successCode = null,
    T? data = default
    ) => CreateResponse<T>(
      context,
      state,
      true,
      message,
      successCode,
      data
      );
}