public interface ISessionRepository
{
  Task AddAsync(Session sess);
  Task UpdateAsync(Session sess);
  Task<IReadOnlyCollection<Session>> GetActiveByUserIdAsync(Guid userId);
  Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken);
  Task<bool> TryUseAndUpdateRefreshTokenAsync(int sessionId);

  Task SetReplacedByAndRevokedAsync(int oldSessionId, int newSessionId);
}