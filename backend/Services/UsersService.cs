using Microsoft.EntityFrameworkCore;

public class UserService
{
  private readonly AppDbContext _context;
  public UserService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<UsersModel>> GetAllUsersAsync()
  {
    var users = await _context.users
    .Include(u => u.Session)
    .Include(u => u.UserData)
    .Include(u => u.Opinions)
    .Include(u => u.ApiLogs)
    .ToListAsync();

    return users;
  }
}