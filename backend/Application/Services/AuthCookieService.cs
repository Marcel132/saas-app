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
      SameSite = SameSiteMode.Lax,
      Path = "/",
      Domain = domain,
      Expires = DateTimeOffset.UtcNow.AddMinutes(15),
      MaxAge = TimeSpan.FromMinutes(15)
    };

    if (isPersistent)
      cookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(7);
      cookieOptions.MaxAge = TimeSpan.FromDays(7);

    return cookieOptions;
  }
  public void SetAuthCookie(HttpResponse response, string? refreshToken, string? authToken)
  {
    if(string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(authToken))
      throw new TokenNotFoundAppException();

    response.Cookies.Append(AuthTokenName, authToken, CreateAuthCookieOptions(false, null, false));

    response.Cookies.Append(RefreshTokenName, refreshToken, CreateAuthCookieOptions(false, null, true));
  }

  public void ClearAuthCookie(HttpResponse response)
  {
    response.Cookies.Delete(AuthTokenName);
    response.Cookies.Delete(RefreshTokenName);
  }

}