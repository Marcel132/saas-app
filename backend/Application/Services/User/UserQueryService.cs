using Microsoft.EntityFrameworkCore;

public class UserQueryService
{
  
  private readonly AppDbContext _context;

  public UserQueryService
  (
    AppDbContext context
  )
  {
    _context = context;
  }

  public async Task<List<UserResponseDto>> GetAllAsync()
  {
    return await _context.Users
      .Select( u => new UserResponseDto
      {
        Id = u.Id,
        Email = u.Email,
        Specialization = u.Specializations.ToList(),
        CreatedAt = u.CreatedAt,
        FirstName = u.UserData.FirstName,
        LastName = u.UserData.LastName,
        Skills = u.UserData.Skills
      })
      .Where(u => u.IsActive)
      .ToListAsync();
  }

   public async Task<UserResponseDto> GetByIdAsync(Guid userId)
  {
    return await _context.Users
      .Where(u => u.Id == userId)
      .Select(u => new UserResponseDto
      {
        Id = u.Id,
        Email = u.Email,
        Specialization = u.Specializations.ToList(),
        FirstName = u.UserData.FirstName,
        LastName = u.UserData.LastName,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
      })
      .FirstOrDefaultAsync()
      ?? throw new KeyNotFoundException();
  }
}