using backend.Domain.Entities;

namespace backend.Domain.Interfaces.Repositories;

public interface ISessionRepository
{
  Task AddAsync(Session sess, CancellationToken ct);
  void Update(Session sess);
  Task<bool> TryMarkSessionAsUsedAsync(long sessionId, CancellationToken ct);
  Task SetReplacedByAndRevokedAsync(long oldSessionId, long newSessionId, CancellationToken ct);
}