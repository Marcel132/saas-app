using backend.Domain.Entities;

namespace backend.Domain.Interfaces.Repositories;

public interface ISessionQueryRepository
{
  Task<Session?> GetActiveSessionAsync(Guid userId, CancellationToken ct);
  Task<IReadOnlyCollection<Session>> GetAllSessionsAsync(Guid userId, CancellationToken ct);
  Task<IReadOnlyCollection<Session>> GetAllActiveSessionsAsync(Guid userId, CancellationToken ct);
  Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken, CancellationToken ct);
  Task<Session?> GetSessionByUserAndIdAsync(Guid userId, long sessionId, CancellationToken ct);
}