using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using backend.Domain.Interfaces.Repositories;

namespace backend.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
  private readonly AppDbContext _context;
  public UserRepository
  (
    AppDbContext context
  )
  {
    _context = context;
  }

  public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
  {
    email = email.Trim().ToLowerInvariant();
    return await _context.Users
      .AsNoTracking()
      .AnyAsync(u => 
        u.Email == email, 
        ct
      );
  }

  public async Task<bool> ExistsByNicknameAsync(string nickname, CancellationToken ct)
  {
    nickname = nickname.Trim().ToLower();

    return await _context.Users
      .AsNoTracking()
      .AnyAsync(u =>
        u.PentesterProfile != null &&
        string.Equals(u.PentesterProfile.NickName.Trim().ToLower(), nickname),
        ct
      );
  }

  public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
  {
    email = email.Trim().ToLowerInvariant();
    return await _context.Users
      .FirstOrDefaultAsync(u => 
        u.Email == email,
        ct
      );
  }

  public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
  {
    return await _context.Users
      .FirstOrDefaultAsync(u => 
        u.Id == id,
        ct  
      );
  }

  public async Task AddAsync(User user, CancellationToken ct)
  {
    await _context.Users.AddAsync(user, ct);
  }

}