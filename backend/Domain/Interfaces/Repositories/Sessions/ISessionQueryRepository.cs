using backend.Domain.Entities;

namespace backend.Domain.Interfaces.Repositories;

public interface ISessionQueryRepository
{
  Task<Session?> GetActiveSessionAsync(Guid userId);
  Task<IReadOnlyCollection<Session>> GetAllSessionsAsync(Guid userId);
  Task<IReadOnlyCollection<Session>> GetAllActiveSessionsAsync(Guid userId);
  Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken);
  Task<Session?> GetSessionByUserAndIdAsync(Guid userId, long sessionId);
}