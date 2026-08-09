namespace backend.Application.Services;

public static class AuthCookies
{
  private const string AuthTokenName = "AuthToken";
  private const string RefreshTokenName = "RefreshToken";
  private const int _jwtTokenExpires = 15; //* IN MINUTES
  private const int _refreshTokenExpires = 7; //* IN DAYS

  public static string? GetRefreshToken (HttpRequest req)
    => req.Cookies[RefreshTokenName];

  private static bool IsHttps (HttpResponse res)
    => res.HttpContext.Request.IsHttps;
    
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
    if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(authToken))
      throw new TokenNotFoundAppException();


    response.Cookies.Append(
      key: AuthTokenName,
      value: authToken,
      options: CreateAuthCookieOptions(
        isHttps: IsHttps(response),
        domain: null,
        isPersistent: false
      )
    );

    response.Cookies.Append(
      key: RefreshTokenName,
      value: refreshToken,
      options: CreateAuthCookieOptions(
        isHttps: IsHttps(response),
        domain: null,
        isPersistent: true
      )
    );
  }

  public static void ClearAuthCookie(HttpResponse response)
  {
    response.Cookies.Delete(AuthTokenName);
    response.Cookies.Delete(RefreshTokenName);
  }

}