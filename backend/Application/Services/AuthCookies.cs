public static class AuthCookies
{
  private const string AuthTokenName = "AuthToken";
  private const string RefreshTokenName = "RefreshToken";
  private const int _jwtTokenExpires = 15; //* IN MINUTES
  private const int _refreshTokenExpires = 7; //* IN DAYS
  public static string? GetRefreshToken(HttpRequest request)
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
      Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtTokenExpires),
      MaxAge = TimeSpan.FromMinutes(_jwtTokenExpires)
    };

    if (isPersistent)
    {
      cookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(_refreshTokenExpires);
      cookieOptions.MaxAge = TimeSpan.FromDays(_refreshTokenExpires);
    }
      

    return cookieOptions;
  }
  public static void SetAuthCookie(HttpResponse response, string? refreshToken, string? authToken)
  {
    if(string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(authToken))
      throw new TokenNotFoundAppException();

    response.Cookies.Append(AuthTokenName, authToken, CreateAuthCookieOptions(false, null, false));

    response.Cookies.Append(RefreshTokenName, refreshToken, CreateAuthCookieOptions(false, null, true));
  }

  public static void ClearAuthCookie(HttpResponse response)
  {
    response.Cookies.Delete(AuthTokenName);
    response.Cookies.Delete(RefreshTokenName);
  }

}