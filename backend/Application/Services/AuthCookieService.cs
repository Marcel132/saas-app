public class AuthCookieService
{
  private const string AuthTokenName = "AuthToken";
  private const string RefreshTokenName = "RefreshToken";
  public AuthCookieService()
  {
  }

  public static CookieOptions CreateAuthCookieOptions(
    bool isHttps,
    string? domain = null, 
    bool isPersistent = false
    )
  {
    var cookieOptions = new CookieOptions
    {
      HttpOnly = true,
      Secure = isHttps,
      SameSite = SameSiteMode.Strict,
      Path = "/",
      Domain = domain,
      Expires = DateTimeOffset.UtcNow.AddHours(15)
    };

    if (isPersistent)
      cookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(7);

    return cookieOptions;
  }

  public void SetAuthCookie(HttpResponse response, string refreshToken, string authToken)
  {
    response.Cookies.Append(AuthTokenName, authToken, CreateAuthCookieOptions(false, null, false));

    response.Cookies.Append(RefreshTokenName, refreshToken, CreateAuthCookieOptions(false, null, true));
  }

  public void ClearAuthCookie(HttpResponse response)
  {
    response.Cookies.Delete(AuthTokenName);
    response.Cookies.Delete(RefreshTokenName);
  }

}