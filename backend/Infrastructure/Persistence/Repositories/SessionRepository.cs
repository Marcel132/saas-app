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
}