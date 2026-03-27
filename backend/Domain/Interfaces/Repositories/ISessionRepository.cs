public interface ISessionRepository
{
  Task<IReadOnlyCollection<Session>> GetActiveByUserIdAsync(Guid userId);
  Task AddAsync(Session sess);
  Task UpdateAsync(Session sess);
  Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken);
  Task<bool> TryUseAndUpdateRefreshTokenAsync(int sessionId);
}