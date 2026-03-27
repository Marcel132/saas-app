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
  public async Task CreateSessionAsync(
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
  }

  public async Task RevokeAllSessionsAsync(Guid userId, int? replacedByTokenId)
  {
    var sessions = await _sessions.GetActiveByUserIdAsync(userId);

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
    var sessions = await _sessions.GetActiveByUserIdAsync(userId);
    var session = sessions.FirstOrDefault(s => s.SessionId == sessionId);

    if (session is null)
      return;

    session.RevokeSession(replacedByTokenId);
    await _sessions.UpdateAsync(session);
  }
  public async Task RevokeActiveSessionAsync(Guid userId, string refreshToken, int? replacedByTokenId)
  {
    var sessions = await _sessions.GetActiveByUserIdAsync(userId);
    var session = sessions.FirstOrDefault(s => BCrypt.Net.BCrypt.Verify(refreshToken, s.RefreshTokenHash));

    if (session is null)
      return;

    session.RevokeSession(replacedByTokenId);
    await _sessions.UpdateAsync(session);
  }
  public async Task<bool> TryUseRefreshTokenAsync(int sessionId)
  {
    var result = await _sessions.TryUseAndUpdateRefreshTokenAsync(sessionId);
    return result;
  }
}
