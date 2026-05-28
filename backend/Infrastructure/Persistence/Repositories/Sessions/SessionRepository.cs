using Microsoft.EntityFrameworkCore;
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

  public async Task AddAsync(Session sess)
  {
    await _context.Sessions.AddAsync(sess);
  }
  public void Update(Session sess)
  {
    _context.Sessions.Update(sess);
  }

  public async Task<bool> TryMarkSessionAsUsedAsync(long sessionId)
  {
    var result =  await _context.Database
      .ExecuteSqlInterpolatedAsync($@"
        UPDATE sessions
        SET used = true
        WHERE id = {sessionId}
          AND used = false
          AND revoked = false
        ");
  
    return result > 0;
  }
  public async Task SetReplacedByAndRevokedAsync(long oldSessionId, long newSessionId)
  {
    await _context.Database
      .ExecuteSqlInterpolatedAsync($@"
        UPDATE sessions
        SET 
          replaced_by_token_id = {newSessionId},
          revoked = true
        WHERE id = {oldSessionId}
          AND replaced_by_token_id IS NULL
        ");
  }
}
