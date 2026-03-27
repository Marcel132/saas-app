public class AuthCookieService
{
  private const string AuthTokenName = "AuthToken";
  private const string RefreshTokenName = "RefreshToken";
  public AuthCookieService()
  {
  }

  public string? GetRefreshToken(HttpRequest request)
  {
    return request.Cookies[RefreshTokenName];
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
      Expires = DateTimeOffset.UtcNow.AddMinutes(15)
    };

    if (isPersistent)
      cookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(5);

    return cookieOptions;
  }
  public void SetAuthCookie(HttpResponse response, string? refreshToken, string? authToken)
  {
    if(string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(authToken))
      throw new ArgumentException("Tokens cannot be null or empty.");

    response.Cookies.Append(AuthTokenName, authToken, CreateAuthCookieOptions(false, null, false));

    response.Cookies.Append(RefreshTokenName, refreshToken, CreateAuthCookieOptions(false, null, true));
  }

  public void ClearAuthCookie(HttpResponse response)
  {
    response.Cookies.Delete(AuthTokenName);
    response.Cookies.Delete(RefreshTokenName);
  }

}