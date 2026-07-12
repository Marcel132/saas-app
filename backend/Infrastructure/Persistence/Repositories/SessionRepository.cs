using backend.Domain.Entities;
using backend.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Persistence.Repositories;

public class SessionRepository : ISessionRepository
{
  private readonly AppDbContext _context;

  public SessionRepository
  (
    AppDbContext context
  )
  {
    _context = context;
  }

  public async Task AddAsync(Session sess, CancellationToken ct)
  {
    await _context.Sessions.AddAsync(sess, ct);
  }
  public void Update(Session sess)
  {
    _context.Sessions.Update(sess);
  }

  public async Task<bool> TryMarkSessionAsUsedAsync(long sessionId, CancellationToken ct)
  {
    var result = await _context.Database
      .ExecuteSqlInterpolatedAsync($@"
        UPDATE sessions
        SET used = true
        WHERE id = {sessionId}
          AND used = false
          AND revoked = false
        ", ct);

    return result > 0;
  }
  public async Task SetReplacedByAndRevokedAsync(long oldSessionId, long newSessionId, CancellationToken ct)
  {
    await _context.Database
      .ExecuteSqlInterpolatedAsync($@"
        UPDATE sessions
        SET 
          replaced_by_token_id = {newSessionId},
          revoked = true
        WHERE id = {oldSessionId}
          AND replaced_by_token_id IS NULL
        ", ct);
  }
}
