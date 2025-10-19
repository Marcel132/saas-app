public class HttpResponseModel<T>
{
  public bool Success { get; set; }
  public HttpResponseState State { get; set; }
  public T? Data { get; set; }
  public string? Message { get; set; }
  public string? TraceId { get; set; }
  public DateTime Timestamp { get; set; } = DateTime.UtcNow;
  
  public string? ErrorCode { get; set; }
}