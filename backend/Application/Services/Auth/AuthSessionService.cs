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
    User user,
    string refreshToken,
    string deviceIp,
    string userAgent
  )
  {
    var session = Session.Create(
      user.Id,
      refreshToken,
      deviceIp,
      userAgent
    );

    await _sessions.AddAsync(session);
  }

  public async Task RevokeAllSessionsAsync(Guid userId)
  {
    var sessions = await _sessions.GetActiveByUserIdAsync(userId);

    foreach (var session in sessions)
    {
      session.RevokeSession();
      await _sessions.UpdateAsync(session);
    }
  }
}