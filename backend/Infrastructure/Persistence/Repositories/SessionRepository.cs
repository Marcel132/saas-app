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
  
  public async Task<Session?> GetActiveSessionAsync(Guid userId)
  {
    return await _context.Sessions
      .Where(s => 
        s.UserId == userId &&
        !s.Revoked &&
        !s.Used
      )
      .FirstOrDefaultAsync();
  }
  public async Task<IReadOnlyCollection<Session>> GetAllSessionsAsync(Guid userId)
  {
    return await _context.Sessions
      .Where(s => s.UserId == userId)
      .ToListAsync();
  }
  public async Task<IReadOnlyCollection<Session>> GetAllActiveSessionsAsync(Guid userId)
  {
    return await _context.Sessions
      .Where(s => 
        s.UserId == userId &&
        !s.Revoked &&
        !s.Used
       )
      .ToListAsync();
  }
  public async Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken)
  {
    var refreshTokenHash = TokenHasher.HashToken(refreshToken);

    return await _context.Sessions
      .Where(s => s.RefreshTokenHash == refreshTokenHash)
      .FirstOrDefaultAsync();
  }
  public async Task<Session?> GetSessionByUserAndIdAsync(Guid userId, int sessionId)
  {
    return await _context.Sessions
      .Where(s => 
        s.UserId == userId &&
        s.SessionId == sessionId
      )
      .FirstOrDefaultAsync();
  }
  public async Task<bool> TryMarkSessionAsUsedAsync(int sessionId)
  {
    var result =  await _context.Database
      .ExecuteSqlInterpolatedAsync($@"
        UPDATE sessions
        SET used = true
        WHERE id = {sessionId}
          AND Used = false
          AND Revoked = false
        ");
  
    await _context.Entry(result).ReloadAsync();
    return result > 0;
  }
  public async Task SetReplacedByAndRevokedAsync(int oldSessionId, int newSessionId)
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
