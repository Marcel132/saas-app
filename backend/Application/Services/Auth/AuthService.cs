public class AuthService
{
  private readonly UserAuthenticationService _authentication;
  private readonly UserRegisterService _registration;
  private readonly AuthSessionService _sessionService;
  private readonly TokenService _tokenService;
  private readonly RoleService _roleService;
  private readonly AuthCookieService _cookieSerivce;
  private readonly RefreshService _refreshService;
  public AuthService(
    AuthSessionService sessionService,
    UserAuthenticationService authenticationService,
    UserRegisterService registerService,

    TokenService tokenService,
    RoleService roleService,
    AuthCookieService authCookieService,
    RefreshService refreshService
  )
  {
    _sessionService = sessionService;
    _authentication = authenticationService;
    _registration = registerService;

    _tokenService = tokenService;
    _roleService = roleService;
    _cookieSerivce = authCookieService;
    _refreshService = refreshService;
  }

  public async Task<AuthResult> LoginAsync(LoginRequestDto request, string deviceIp, string userAgent, HttpResponse response)
  {
    var lowerCaseEmail = request.Email.Trim().ToLowerInvariant();

    var user = await _authentication.AuthenticateAsync(
      lowerCaseEmail,
      request.Password
    );

    var permissions = await _roleService.GetEffectivePermissions(user.Id);

    var authToken = _tokenService.GenerateAuthToken(user.Id, permissions);
    var refreshToken = _tokenService.GenerateRefreshToken();


    await _sessionService.CreateSessionAsync(user.Id, refreshToken, deviceIp, userAgent);

    _cookieSerivce.SetAuthCookie(response, refreshToken, authToken);

    return new AuthResult(
      true,
      user.Id,
      permissions, 
      DomainErrorCodes.AuthCodes.Authorized
    );
  }

  public async Task<AuthResult> RegisterAsync(RegisterRequestDto request, string deviceIp, string userAgent, HttpResponse response)
  {
    
    var user = await _registration.RegisterAsync(request);

    var permissions = await _roleService.GetEffectivePermissions(user.Id);

    var authToken = _tokenService.GenerateAuthToken(user.Id, permissions);
    var refreshToken = _tokenService.GenerateRefreshToken();

    await _sessionService.CreateSessionAsync(user.Id, refreshToken, deviceIp, userAgent);

    _cookieSerivce.SetAuthCookie(response, refreshToken, authToken);


    return new AuthResult(
      true,
      user.Id,
      permissions, // session.Permission
      DomainErrorCodes.AuthCodes.Authorized
    );
  }

  public async Task LogoutAsync(Guid userId, HttpResponse response)
  {
    await _sessionService.RevokeAllSessionsAsync(userId);
    _cookieSerivce.ClearAuthCookie(response);
  }

  public async Task<RefreshTokenResult> RefreshTokenAsync(string deviceIp, string userAgent, string? refreshToken)
  {
    if(string.IsNullOrEmpty(refreshToken))
    {
      return new RefreshTokenResult(
        false,
        Guid.Empty,
        null,
        DomainErrorCodes.AuthCodes.InvalidToken,
        null,
        null
      );
    }
    
    var result = await _refreshService.ValidateRefreshTokenAsync(refreshToken, deviceIp, userAgent);

    if(!result.Success || result.session is null)
      return result;

    var newRefreshToken = _tokenService.GenerateRefreshToken();
    await _sessionService.CreateSessionAsync(result.UserId, newRefreshToken, deviceIp, userAgent );

    await _sessionService.RevokeSessionByIdAsync(result.UserId, result.session.SessionId);
    var permissions = await _roleService.GetEffectivePermissions(result.UserId);
    var authToken = _tokenService.GenerateAuthToken(result.UserId, permissions);


    if(string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(newRefreshToken))
    {
      return new RefreshTokenResult(
        false,
        result.UserId,
        null,
        DomainErrorCodes.GeneralCodes.GenerationFailed,
        null,
        null
      );
    }
    
    return new RefreshTokenResult(
      true,
      result.UserId,
      null,
      DomainErrorCodes.AuthCodes.Authorized,
      newRefreshToken,
      authToken
    );
  }
}