using backend.Api.Controllers.Auth.DTOs;
using backend.Application.Services.Auth.DTOs;
using backend.Domain.Interfaces.Features;

namespace backend.Application.Services;

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

  public async Task<CredentialsDto> RegisterPentesterAsync(RegisterPentesterRequestDto req, string ipAddress, string userAgent)
  {
    var user = await _registration.RegisterPentesterAsync(req);

    return await GenerateCredentialsAndHandleSessionAsync(user.Id, ipAddress, userAgent);
  }

  public async Task<CredentialsDto> RegisterCompanyAsync(RegisterCompanyRequestDto req, string ipAddress, string userAgent)
  {
    var user = await _registration.RegisterCompanyAsync(req);

    return await GenerateCredentialsAndHandleSessionAsync(user.Id, ipAddress, userAgent);
  }

  // TODO: Logout user per ip and device instead of logging out from all sessions to prevent session hijacking
  public async Task LogoutAsync(Guid userId)
  {
    await _sessionService.RevokeAllSessionsAsync(userId, null);
  }

  public async Task<CredentialsDto> RefreshTokenAsync(string ipAddress, string userAgent, string? refreshToken)
  {
    if (string.IsNullOrEmpty(refreshToken))
      throw new TokenNotFoundAppException();

    var result = await RotateRefreshTokenAsync(refreshToken, ipAddress, userAgent);

    return result;
  }

  public async Task<CredentialsDto> GenerateCredentialsAndHandleSessionAsync(Guid userId, string ipAddress, string userAgent)
  {
    var tokens = await _credentialsService.GenerateCredentials(userId);

    // TODO: Instead of revoking all sessions, we should revoke only the session from the current device and ip to prevent session hijacking. This requires implementing device and ip tracking in sessions.
    await _sessionService.RevokeAllSessionsAsync(userId, null);
    await _sessionService.CreateSessionAsync(userId, tokens.RefreshToken, ipAddress, userAgent);

    return tokens;
  }
  private async Task<CredentialsDto> RotateRefreshTokenAsync(string refreshToken, string ipAddress, string userAgent)
  {
    var validationResult = await ValidateRefreshTokenAsync(refreshToken, ipAddress, userAgent);

    var used = await _sessionService.TryUseRefreshTokenAsync(validationResult.Session.Id);
    if (!used)
    {
      await _sessionService.RevokeAllSessionsAsync(validationResult.UserId, validationResult.Session.Id);
      throw new SuspiciousActivityAppException();
    }

    var tokens = await _credentialsService.GenerateCredentials(validationResult.UserId);
    var newSession = await _sessionService.CreateSessionAsync(validationResult.UserId, tokens.RefreshToken, ipAddress, userAgent);

    await _sessionService.SetReplacedByAndRevokedAsync(validationResult.Session.Id, newSession.Id);

    return tokens;
  }

  private async Task<ValidateSession> ValidateRefreshTokenAsync(string refreshToken, string? ipAddress, string? userAgent)
  {
    var session = await _sessionService.GetSessionByRefreshTokenAsync(refreshToken)
      ?? throw new SessionNotFoundAppException();

    if (session.Revoked || session.Used)
    {
      await _sessionService.RevokeAllSessionsAsync(session.UserId, null);
      throw new SuspiciousActivityAppException("Token used is flagged: Revoked and Used");
    }

    if (session.ExpiresAt <= DateTime.UtcNow)
    {
      await _sessionService.RevokeSessionByIdAsync(session.UserId, session.Id, null);
      throw new TokenExpiredAppException();
    }

    return new ValidateSession
    {
      UserId = session.UserId,
      Session = session
    };
  }

}