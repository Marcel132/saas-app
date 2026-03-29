using Microsoft.EntityFrameworkCore;
using Npgsql;

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

  public async Task<IReadOnlyCollection<Session>> GetActiveByUserIdAsync(Guid userId)
  {
    return await _context.Sessions
      .Where(s =>
        s.UserId == userId &&
        !s.Revoked
      )
      .ToListAsync();
  }
  public async Task AddAsync(Session sess)
  {
    _context.Sessions.Add(sess);
    await _context.SaveChangesAsync();
  }
  public async Task UpdateAsync(Session sess)
  {
    _context.Sessions.Update(sess);
    await _context.SaveChangesAsync();
  }
  public async Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken)
  {
    var shaToken = TokenHasher.HashToken(refreshToken);
    return await _context.Sessions
      .Where(s => s.RefreshTokenHash == shaToken)
      .FirstOrDefaultAsync();
  }
  public async Task<bool> TryUseAndUpdateRefreshTokenAsync(int sessionId)
  {
    var result =  await _context.Database
      .ExecuteSqlInterpolatedAsync($@"
        UPDATE sessions
        SET used = true
        WHERE id = {sessionId}
          AND Used = false
          AND Revoked = false
        ");
  
    return result > 0;
  }

  public async Task SetReplacedByAndRevokedAsync(int oldSessionId, int newSessionId)
  {
    await _context.Database
      .ExecuteSqlInterpolatedAsync($@"
        UPDATE sessions
        SET replaced_by_token_id = {newSessionId},
          revoked = true
        WHERE id = {oldSessionId}
          AND replaced_by_token_id IS NULL
        ");
  }
}
