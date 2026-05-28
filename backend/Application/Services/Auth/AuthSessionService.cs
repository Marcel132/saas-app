public class AuthSessionService
{
  private readonly ISessionRepository _sessionRepository;
  private readonly ISessionQueryRepository _sessionQueryRepository;
  private readonly IUnitOfWork _unitOfWork;

  public AuthSessionService
  (
    ISessionRepository sessionRepository,
    ISessionQueryRepository sessionQueryRepository,
    IUnitOfWork unitOfWork
  )
  {
    _sessionRepository = sessionRepository;
    _sessionQueryRepository = sessionQueryRepository;
    _unitOfWork = unitOfWork;
  }
  public async Task<Session> CreateSessionAsync(
    Guid userId,
    string refreshToken,
    string deviceIp,
    string userAgent
  )
  {
    var session = Session.Create(
      userId,
      refreshToken,
      userAgent,
      deviceIp
    );

    await _sessionRepository.AddAsync(session);
    await _unitOfWork.SaveChangesAsync();

    return session;
  }



  public async Task RevokeAllSessionsAsync(Guid userId, long? replacedByTokenId)
  {
    var sessions = await _sessionQueryRepository.GetAllActiveSessionsAsync(userId);

    foreach (var session in sessions)
    {
      session.RevokeSession(replacedByTokenId);
      _sessionRepository.Update(session);
    }
    await _unitOfWork.SaveChangesAsync();
  }

  public async Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken)
  {
    return await _sessionQueryRepository.GetSessionByRefreshTokenAsync(refreshToken);
  }
  public async Task RevokeSessionByIdAsync(Guid userId, long sessionId, long? replacedByTokenId)
  {
    var session = await _sessionQueryRepository.GetSessionByUserAndIdAsync(userId, sessionId)
      ?? throw new SessionNotFoundAppException();

    session.RevokeSession(replacedByTokenId);
    _sessionRepository.Update(session);
    await _unitOfWork.SaveChangesAsync();
  }
  public async Task RevokeActiveSessionAsync(string refreshToken, long? replacedByTokenId)
  {
    var session = await _sessionQueryRepository.GetSessionByRefreshTokenAsync(refreshToken);

    if (session is null)
      return;

    session.RevokeSession(replacedByTokenId);
    _sessionRepository.Update(session);
    await _unitOfWork.SaveChangesAsync();
  }
  public async Task<bool> TryUseRefreshTokenAsync(long sessionId)
  {
    var result = await _sessionRepository.TryMarkSessionAsUsedAsync(sessionId);
    return result;
  }

  public async Task SetReplacedByAndRevokedAsync(long oldSessionId, long newSessionId)
  {
    await _sessionRepository.SetReplacedByAndRevokedAsync(oldSessionId, newSessionId);
  }
}
