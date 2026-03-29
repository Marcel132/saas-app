using System.Drawing;

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

  // TODO: Logout user per ip and device instead of logging out from all sessions to prevent session hijacking
  public async Task LogoutAsync(Guid userId, HttpResponse response)
  {
    await _sessionService.RevokeAllSessionsAsync(userId, null);
    _cookieSerivce.ClearAuthCookie(response);
  }

  public async Task<RefreshTokenResult> RefreshTokenAsync(string deviceIp, string userAgent, string? refreshToken)
  {
    if(string.IsNullOrEmpty(refreshToken))
      throw new TokenNotFoundAppException();
      
    var result = await RotateRefreshTokenAsync(refreshToken, deviceIp, userAgent);
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

  private async Task<RefreshTokenResult> RotateRefreshTokenAsync(string refreshToken, string deviceIp, string userAgent)
  {
    var validationResult = await _refreshService.ValidateRefreshTokenAsync(refreshToken, deviceIp, userAgent);
    if(!validationResult.Success || validationResult.session is null)
      throw new SessionNotFoundAppException();
    
    var used = await _sessionService.TryUseRefreshTokenAsync(validationResult.session.SessionId);
    if (!used)
    {
      await _sessionService.RevokeAllSessionsAsync(validationResult.UserId, validationResult.session.SessionId);
      throw new SuspiciousActivityAppException();
    }
    
    var newRefreshToken = _tokenService.GenerateRefreshToken();
    var newSession =await _sessionService.CreateSessionAsync(validationResult.UserId, newRefreshToken, deviceIp, userAgent);

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

}