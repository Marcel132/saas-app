namespace backend.Api.Middlewares.Helpers;

public class RateLimitBucket
{
  public Double Token { get; set; }
  public DateTime LastRefill { get; set; }

}