using backend.Domain.Entities;

namespace backend.Domain.Interfaces.Repositories;

public interface ISessionRepository
{
  Task AddAsync(Session sess);
  void Update(Session sess);
  Task<bool> TryMarkSessionAsUsedAsync(long sessionId);
  Task SetReplacedByAndRevokedAsync(long oldSessionId, long newSessionId);
}