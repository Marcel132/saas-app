using backend.Domain.Entities;

namespace backend.Domain.Interfaces.Features;
public interface IAuthSessioonService
{
  public Task<Session> CreateSessionAsync(Guid userId, string refreshToken, string deviceIp, string userAgent, CancellationToken ct);
  public Task RevokeAllSessionsAsync(Guid userId, long?  replaceByTokenId, CancellationToken ct);
  public Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken, CancellationToken ct);
  public Task RevokeSessionByIdAsync(Guid userId, long SessionId, long? replacedByTokenId, CancellationToken ct);
  public Task RevokeActiveSessionAsync(string refreshToken, long? replacedByTokenId, CancellationToken ct);
  public Task<bool> TryUseRefreshTokenAsync(long sessionId, CancellationToken ct);
  public Task SetReplacedByAndRevokedAsync(long oldSessionId, long newSessionId, CancellationToken ct);
}