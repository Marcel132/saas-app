public class AuthSessionService
{
  private readonly ISessionRepository _sessions;

  public AuthSessionService
  (
    ISessionRepository sessions
  )
  {
    _sessions = sessions;
  }
  public async Task<Session> CreateSessionAsync(
    Guid user_id,
    string refreshToken,
    string deviceIp,
    string userAgent
  )
  {
    var session = Session.Create(
      user_id,
      refreshToken,
      deviceIp,
      userAgent
    );

    await _sessions.AddAsync(session);

    return session;
  }



  public async Task RevokeAllSessionsAsync(Guid userId, int? replacedByTokenId)
  {
    var sessions = await _sessions.GetAllActiveSessionsAsync(userId);

    foreach (var session in sessions)
    {
      session.RevokeSession(replacedByTokenId);
      await _sessions.UpdateAsync(session);
    }
  }

  public async Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken)
  {
    return await _sessions.GetSessionByRefreshTokenAsync(refreshToken);
  }
  public async Task RevokeSessionByIdAsync(Guid userId, int sessionId, int? replacedByTokenId)
  {
    var session = await _sessions.GetSessionByUserAndIdAsync(userId, sessionId)
      ?? throw new SessionNotFoundAppException();

    session.RevokeSession(replacedByTokenId);
    await _sessions.UpdateAsync(session);
  }
  public async Task RevokeActiveSessionAsync(string refreshToken, int? replacedByTokenId)
  {
    var session = await _sessions.GetSessionByRefreshTokenAsync(refreshToken);

    if (session is null)
      return;

    session.RevokeSession(replacedByTokenId);
    await _sessions.UpdateAsync(session);
  }
  public async Task<bool> TryUseRefreshTokenAsync(int sessionId)
  {
    var result = await _sessions.TryMarkSessionAsUsedAsync(sessionId);
    return result;
  }

  public async Task SetReplacedByAndRevokedAsync(int oldSessionId, int newSessionId)
  {
    await _sessions.SetReplacedByAndRevokedAsync(oldSessionId, newSessionId);
  }
}
