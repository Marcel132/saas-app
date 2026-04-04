using Microsoft.EntityFrameworkCore;

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



  public async Task<bool> ExistsByEmailAsync(string email)
  {
    email = email.Trim().ToLowerInvariant();
    return await _context.Users
      .AsNoTracking()
      .AnyAsync(u => u.Email == email);
  }
  
  public async Task<User?> GetByEmailAsync(string email)
  {
    return await _context.Users
      .FirstOrDefaultAsync(u => u.Email == email);
  }
  
  public async Task<User?> GetByIdAsync(Guid id)
  {
    return await _context.Users
      .FirstOrDefaultAsync(u => u.Id == id);
  }

  public async Task AddAsync(User user)
  {
    await _context.Users.AddAsync(user);
  }

  public async Task UpdateAsync(User user)
  {
    _context.Users.Update(user);
  }

  public async Task DeleteAsync(User user)
  {
    _context.Users.Remove(user);
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }

}