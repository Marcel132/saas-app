using backend.Domain.Entities;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Auth.Shared;

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
    string userAgent,
    CancellationToken ct
  )
  {
    var session = Session.Create(
      userId,
      refreshToken,
      userAgent,
      deviceIp
    );

    await _sessionRepository.AddAsync(session, ct);
    await _unitOfWork.SaveChangesAsync(ct);

    return session;
  }



  public async Task RevokeAllSessionsAsync(Guid userId, long? replacedByTokenId, CancellationToken ct)
  {
    var sessions = await _sessionQueryRepository.GetAllActiveSessionsAsync(userId, ct);

    foreach (var session in sessions)
    {
      session.RevokeSession(replacedByTokenId);
      _sessionRepository.Update(session);
    }
    await _unitOfWork.SaveChangesAsync(ct);
  }

  public async Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken, CancellationToken ct)
  {
    return await _sessionQueryRepository.GetSessionByRefreshTokenAsync(refreshToken, ct);
  }
  public async Task RevokeSessionByIdAsync(Guid userId, long sessionId, long? replacedByTokenId, CancellationToken ct)
  {
    var session = await _sessionQueryRepository.GetSessionByUserAndIdAsync(userId, sessionId, ct)
      ?? throw new SessionNotFoundAppException();

    session.RevokeSession(replacedByTokenId);
    _sessionRepository.Update(session);
    await _unitOfWork.SaveChangesAsync(ct);
  }
  public async Task RevokeActiveSessionAsync(string refreshToken, long? replacedByTokenId, CancellationToken ct)
  {
    var session = await _sessionQueryRepository.GetSessionByRefreshTokenAsync(refreshToken, ct);

    if (session is null)
      return;

    session.RevokeSession(replacedByTokenId);
    _sessionRepository.Update(session);
    await _unitOfWork.SaveChangesAsync(ct);
  }
  public async Task<bool> TryUseRefreshTokenAsync(long sessionId, CancellationToken ct)
  {
    return await _sessionRepository.TryMarkSessionAsUsedAsync(sessionId, ct);
  }

  public async Task SetReplacedByAndRevokedAsync(long oldSessionId, long newSessionId, CancellationToken ct)
  {
    await _sessionRepository.SetReplacedByAndRevokedAsync(oldSessionId, newSessionId, ct);
  }
}
