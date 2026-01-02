public interface ISessionRepository
{
  Task<IReadOnlyCollection<Session>> GetActiveByUserIdAsync(Guid userId);
  Task AddAsync(Session sess);
  Task UpdateAsync(Session sess);
}