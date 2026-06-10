public class AuthService
{
  private readonly UserAuthenticationService _authentication;
  private readonly UserRegisterService _registration;
  private readonly AuthSessionService _sessionService;
  private readonly TokenService _tokenService;
  private readonly RoleService _roleService;
  private readonly AuthCookieService _cookieService;
  public AuthService(
    AuthSessionService sessionService,
    UserAuthenticationService authenticationService,
    UserRegisterService registerService,

    TokenService tokenService,
    RoleService roleService,
    AuthCookieService authCookieService
  )
  {
    _sessionService = sessionService;
    _authentication = authenticationService;
    _registration = registerService;

    _tokenService = tokenService;
    _roleService = roleService;
    _cookieService = authCookieService;
  }

  public async Task<AuthResult> LoginAsync(LoginRequestDto request, string ipAddress, string userAgent, HttpResponse response)
  {
    var user = await _authentication.AuthenticateAsync(
      request.Email,
      request.Password
    );

    var permissions = await _roleService.GetEffectivePermissions(user.Id);

    var authToken = _tokenService.GenerateAuthToken(user.Id, permissions);
    var refreshToken = _tokenService.GenerateRefreshToken();

    // TODO: Instead of revoking all sessions, we should revoke only the session from the current device and ip to prevent session hijacking. This requires implementing device and ip tracking in sessions.
    await _sessionService.RevokeAllSessionsAsync(user.Id, null);
    await _sessionService.CreateSessionAsync(user.Id, refreshToken, ipAddress, userAgent);

    _cookieService.SetAuthCookie(response, refreshToken, authToken);

    return new AuthResult(
      true,
      user.Id,
      permissions, 
      DomainErrorCodes.AuthCodes.Authorized
    );
  }

  public async Task<AuthResult> RegisterAsync(RegisterRequestDto request, string ipAddress, string userAgent, HttpResponse response)
  {
    
    var user = await _registration.RegisterAsync(request);

    var permissions = await _roleService.GetEffectivePermissions(user.Id);

    var authToken = _tokenService.GenerateAuthToken(user.Id, permissions);
    var refreshToken = _tokenService.GenerateRefreshToken();

    // TODO: Instead of revoking all sessions, we should revoke only the session from the current device and ip to prevent session hijacking. This requires implementing device and ip tracking in sessions.
    await _sessionService.RevokeAllSessionsAsync(user.Id, null);
    await _sessionService.CreateSessionAsync(user.Id, refreshToken, ipAddress, userAgent);

    _cookieService.SetAuthCookie(response, refreshToken, authToken);


    return new AuthResult(
      true,
      user.Id,
      permissions, // session.Permission
      DomainErrorCodes.AuthCodes.Authorized
    );
  }

  // TODO: Logout user per ip and device instead of logging out from all sessions to prevent session hijacking
  public async Task LogoutAsync(Guid userId, HttpResponse response)
  {
    await _sessionService.RevokeAllSessionsAsync(userId, null);
    _cookieService.ClearAuthCookie(response);
  }

  public async Task<RefreshTokenResult> RefreshTokenAsync(string ipAddress, string userAgent, string? refreshToken)
  {
    if(string.IsNullOrEmpty(refreshToken))
      throw new TokenNotFoundAppException();
      
    var result = await RotateRefreshTokenAsync(refreshToken, ipAddress, userAgent);
    if(!result.Success)
      return new RefreshTokenResult(
        false,
        result.UserId,
        null,
        result.DomainCode,
        null, 
        null
      );

    return new RefreshTokenResult(
      result.Success,
      result.UserId,
      result.session,
      result.DomainCode,
      result.RefreshToken, //New refresh token
      result.AuthToken //New auth token
    );
  }

  private async Task<RefreshTokenResult> RotateRefreshTokenAsync(string refreshToken, string ipAddress, string userAgent)
  {
    var validationResult = await ValidateRefreshTokenAsync(refreshToken, ipAddress, userAgent);
    if(!validationResult.Success || validationResult.session is null)
      throw new SessionNotFoundAppException();
    
    var used = await _sessionService.TryUseRefreshTokenAsync(validationResult.session.SessionId);
    if (!used)
    {
      await _sessionService.RevokeAllSessionsAsync(validationResult.UserId, validationResult.session.SessionId);
      throw new SuspiciousActivityAppException();
    }
    
    var newRefreshToken = _tokenService.GenerateRefreshToken();
    var newSession = await _sessionService.CreateSessionAsync(validationResult.UserId, newRefreshToken, ipAddress, userAgent);

    await _sessionService.SetReplacedByAndRevokedAsync(validationResult.session.SessionId, newSession.SessionId);

    var permissions = await _roleService.GetEffectivePermissions(validationResult.UserId);
    var authToken = _tokenService.GenerateAuthToken(validationResult.UserId, permissions)
      ?? throw new InternalServerAppException();

    return new RefreshTokenResult(
      true,
      validationResult.UserId,
      null,
      DomainErrorCodes.AuthCodes.Authorized,
      newRefreshToken,
      authToken
    );
  }

  private async Task<RefreshTokenResult> ValidateRefreshTokenAsync(string refreshToken, string ipAddress, string userAgent)
  {
    var session = await _sessionService.GetSessionByRefreshTokenAsync(refreshToken);

    if(session == null)
      return new RefreshTokenResult(
        false,
        Guid.Empty,
        null,
        DomainErrorCodes.AuthCodes.SessionNotFound,
        null, 
        null
      );
    
    if(session.Revoked || session.Used)
    {
      await _sessionService.RevokeAllSessionsAsync(session.UserId, null);
      return new RefreshTokenResult(
        false,
        session.UserId,
        null,
        DomainErrorCodes.FirewallCodes.SuspiciousActivityDetected,
        null,
        null
      );
    }

    if(session.ExpiresAt <= DateTime.UtcNow)
    {
      await _sessionService.RevokeSessionByIdAsync(session.UserId, session.SessionId, null);
      return new RefreshTokenResult(
        false,
        session.UserId,
        null,
        DomainErrorCodes.AuthCodes.TokenExpired,
        null,
        null
      );
    }


    return new RefreshTokenResult(
      true,
      session.UserId,
      session,
      DomainErrorCodes.AuthCodes.ValidToken,
      null,
      null
    );
  }

}