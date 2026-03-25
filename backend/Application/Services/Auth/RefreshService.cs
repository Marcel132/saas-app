public class RefreshService
{
  private readonly AuthSessionService _sessionService;

  public RefreshService
  (
    AuthSessionService sessionService
  )
  {
    _sessionService = sessionService;

  }

  public async Task<RefreshTokenResult> ValidateRefreshTokenAsync(string refreshToken, string deviceIp, string userAgent)
  {
    var session = await _sessionService.GetSessionByRefreshTokenAsync(refreshToken);

    if(session == null)
    {
      return new RefreshTokenResult(
        false,
        Guid.Empty,
        null,
        DomainErrorCodes.AuthCodes.SessionNotFound,
        null, 
        null
      );
    }

    if(session.Revoked)
    {
      await _sessionService.RevokeAllSessionsAsync(session.UserId);
      return new RefreshTokenResult(
        false,
        session.UserId,
        null,
        DomainErrorCodes.FirewallCodes.SuspiciousActivityDetected,
        null,
        null
      );
    }

    if(session.ExpiresAt < DateTime.UtcNow)
    {
      await _sessionService.RevokeSessionByIdAsync(session.UserId, session.SessionId);
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