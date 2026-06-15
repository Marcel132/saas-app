using Microsoft.AspNetCore.Http.HttpResults;

public class AuthService : IAuthService
{
  private readonly UserAuthenticationService _authentication;
  private readonly UserRegisterService _registration;
  private readonly AuthSessionService _sessionService;
  private readonly ICredentialsService _credentialsService;

  public AuthService(
    AuthSessionService sessionService,
    UserAuthenticationService authenticationService,
    UserRegisterService registerService,
    ICredentialsService credentialsService
  )
  {
    _sessionService = sessionService;
    _authentication = authenticationService;
    _registration = registerService;
    _credentialsService = credentialsService;
  }

  public async Task<CredentialsDto> LoginAsync(LoginRequestDto request, string ipAddress, string userAgent)
  {
    var user = await _authentication.AuthenticateAsync(
      request.Email,
      request.Password
    );

    var tokens = await _credentialsService.GenerateCredentials(user.Id);

    // TODO: Instead of revoking all sessions, we should revoke only the session from the current device and ip to prevent session hijacking. This requires implementing device and ip tracking in sessions.
    await _sessionService.RevokeAllSessionsAsync(user.Id, null);
    await _sessionService.CreateSessionAsync(user.Id, tokens.RefreshToken, ipAddress, userAgent);

    return new CredentialsDto
    {
      AuthToken = tokens.AuthToken,
      RefreshToken = tokens.RefreshToken
    };
  }

  public async Task<CredentialsDto> RegisterAsync(RegisterRequestDto request, string ipAddress, string userAgent)
  {
    
    var user = await _registration.RegisterAsync(request);

    var tokens = await _credentialsService.GenerateCredentials(user.Id);

    // TODO: Instead of revoking all sessions, we should revoke only the session from the current device and ip to prevent session hijacking. This requires implementing device and ip tracking in sessions.
    await _sessionService.RevokeAllSessionsAsync(user.Id, null);
    await _sessionService.CreateSessionAsync(user.Id, tokens.RefreshToken, ipAddress, userAgent);

    return new CredentialsDto
    {
      AuthToken = tokens.AuthToken,
      RefreshToken = tokens.RefreshToken
    };
  }

  // TODO: Logout user per ip and device instead of logging out from all sessions to prevent session hijacking
  public async Task LogoutAsync(Guid userId)
  {
    await _sessionService.RevokeAllSessionsAsync(userId, null);
  }

  public async Task<CredentialsDto> RefreshTokenAsync(string ipAddress, string userAgent, string? refreshToken)
  {
    if(string.IsNullOrEmpty(refreshToken))
      throw new TokenNotFoundAppException();
      
    var result = await RotateRefreshTokenAsync(refreshToken, ipAddress, userAgent);

    return result;
  }

  private async Task<CredentialsDto> RotateRefreshTokenAsync(string refreshToken, string ipAddress, string userAgent)
  {
    var validationResult = await ValidateRefreshTokenAsync(refreshToken, ipAddress, userAgent);
    
    var used = await _sessionService.TryUseRefreshTokenAsync(validationResult.Session.SessionId);
    if (!used)
    {
      await _sessionService.RevokeAllSessionsAsync(validationResult.UserId, validationResult.Session.SessionId);
      throw new SuspiciousActivityAppException();
    }
    
    var tokens = await _credentialsService.GenerateCredentials(validationResult.UserId);
    var newSession = await _sessionService.CreateSessionAsync(validationResult.UserId, tokens.RefreshToken, ipAddress, userAgent);

    await _sessionService.SetReplacedByAndRevokedAsync(validationResult.Session.SessionId, newSession.SessionId);

    return new CredentialsDto
    {
      AuthToken = tokens.AuthToken,
      RefreshToken = tokens.RefreshToken
    };
  }

  private async Task<ValidateSession> ValidateRefreshTokenAsync(string refreshToken, string? ipAddress, string? userAgent)
  {
    var session = await _sessionService.GetSessionByRefreshTokenAsync(refreshToken)
      ?? throw new SessionNotFoundAppException();

    if(session.Revoked || session.Used)
    {
      await _sessionService.RevokeAllSessionsAsync(session.UserId, null);
      throw new SuspiciousActivityAppException("Token used is flagged: Revoked and Used");
    }

    if(session.ExpiresAt <= DateTime.UtcNow)
    {
      await _sessionService.RevokeSessionByIdAsync(session.UserId, session.SessionId, null);
      throw new TokenExpiredAppException();
    }

    return new ValidateSession
    {
      UserId = session.UserId,
      Session = session
    };
  }

}