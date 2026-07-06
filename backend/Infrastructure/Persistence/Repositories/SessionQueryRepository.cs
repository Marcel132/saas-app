using backend.Domain.Entities;
using backend.Domain.Interfaces.Repositories;
using backend.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Persistence.Repositories;

public class SessionQueryRepository : ISessionQueryRepository
{
  private readonly AppDbContext _context;
  public SessionQueryRepository(AppDbContext context)
  {
    _context = context;
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
  public async Task<Session?> GetSessionByUserAndIdAsync(Guid userId, long sessionId)
  {
    return await _context.Sessions
      .Where(s =>
        s.UserId == userId &&
        s.Id == sessionId
      )
      .FirstOrDefaultAsync();
  }

}