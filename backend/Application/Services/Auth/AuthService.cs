public class AuthService
{
  private readonly IRegisterPolicy _policy;
  private readonly UserAuthenticationService _authentication;
  private readonly UserRegisterService _registration;
  private readonly AuthSessionService _sessionService;
  private readonly TokenService _tokenService;
  private readonly RoleService _roleService;
  private readonly AuthCookieService _cookieSerivce;
  public AuthService(
    IRegisterPolicy policy,
    AuthSessionService sessionService,
    UserAuthenticationService authenticationService,
    UserRegisterService registerService,

    TokenService tokenService,
    RoleService roleService,
    AuthCookieService authCookieService
  )
  {
    _policy = policy;
    _sessionService = sessionService;
    _authentication = authenticationService;
    _registration = registerService;

    _tokenService = tokenService;
    _roleService = roleService;
    _cookieSerivce = authCookieService;
  }

  public async Task<AuthResult> LoginAsync(LoginRequestDto request, string deviceIp, string userAgent, HttpResponse response)
  {
    var lowerCaseEmail = request.Email.Trim().ToLowerInvariant();

    var user = await _authentication.AuthenticateAsync(
      lowerCaseEmail,
      request.Password
    );

    var permissions = await _roleService.GetEffectivePermissions(user.Id);

    var tokens = _tokenService.GenerateAuthToken(user.Id, permissions);

    await _sessionService.CreateSessionAsync(user, tokens.RefreshToken, deviceIp, userAgent);

    _cookieSerivce.SetAuthCookie(response, tokens.RefreshToken, tokens.AuthToken);

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

    var tokens = _tokenService.GenerateAuthToken(user.Id, permissions);

    await _sessionService.CreateSessionAsync(user, tokens.RefreshToken, deviceIp, userAgent);

    _cookieSerivce.SetAuthCookie(response, tokens.RefreshToken, tokens.AuthToken);


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

}