public interface ISessionRepository
{
  Task AddAsync(Session sess);
  Task UpdateAsync(Session sess);

  Task<Session?> GetActiveSessionAsync(Guid userId);
  Task<IReadOnlyCollection<Session>> GetAllSessionsAsync(Guid userId);
  Task<IReadOnlyCollection<Session>> GetAllActiveSessionsAsync(Guid userId);
  Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken);
  Task<Session?> GetSessionByUserAndIdAsync(Guid userId, long sessionId);
  Task<bool> TryMarkSessionAsUsedAsync(long sessionId);
  Task SetReplacedByAndRevokedAsync(long oldSessionId, long newSessionId);
}