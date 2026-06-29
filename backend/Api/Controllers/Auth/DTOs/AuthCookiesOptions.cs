namespace backend.Api.Controllers.Auth.DTOs;

public class AuthCookiesOptions
{
  public int AuthTokenLifetimeInMinutes { get; set; } = 15;
  public int RefreshTokenLifeTimeInDays { get; set; } = 7;
}