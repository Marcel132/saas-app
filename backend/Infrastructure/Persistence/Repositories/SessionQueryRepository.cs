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

  public async Task<Session?> GetActiveSessionAsync(Guid userId, CancellationToken ct)
  {
    return await _context.Sessions
      .Where(s =>
        s.UserId == userId &&
        !s.Revoked &&
        !s.Used
      )
      .FirstOrDefaultAsync(ct);
  }
  public async Task<IReadOnlyCollection<Session>> GetAllSessionsAsync(Guid userId, CancellationToken ct)
  {
    return await _context.Sessions
      .Where(s => s.UserId == userId)
      .ToListAsync(ct);
  }
  public async Task<IReadOnlyCollection<Session>> GetAllActiveSessionsAsync(Guid userId, CancellationToken ct)
  {
    return await _context.Sessions
      .Where(s =>
        s.UserId == userId &&
        !s.Revoked &&
        !s.Used
       )
      .ToListAsync(ct);
  }
  public async Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken, CancellationToken ct)
  {
    var refreshTokenHash = TokenHasher.HashToken(refreshToken);

    return await _context.Sessions
      .Where(s => s.RefreshTokenHash == refreshTokenHash)
      .FirstOrDefaultAsync(ct);
  }
  public async Task<Session?> GetSessionByUserAndIdAsync(Guid userId, long sessionId, CancellationToken ct)
  {
    return await _context.Sessions
      .Where(s =>
        s.UserId == userId &&
        s.Id == sessionId
      )
      .FirstOrDefaultAsync(ct);
  }

}