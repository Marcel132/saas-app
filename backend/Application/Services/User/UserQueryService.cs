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

  public async Task<PagedResponse<UserResponseDto>> GetAllAsync(int page, int pageSize, string? search = null)
  {
    page = Math.Max(page, 1);
    pageSize = Math.Clamp(pageSize, 1, 50);
    var query = _context.Users
      .AsNoTracking()
      .Where(u => u.IsActive);

    if (!string.IsNullOrEmpty(search))
    {
      query = query
        .Where(u => 
        EF.Functions.ILike(u.Email, $"%{search}%") || 
        EF.Functions.ILike(u.UserData.FirstName, $"%{search}%") || 
        EF.Functions.ILike(u.UserData.LastName, $"%{search}%"));
    }

    var totalItems = await query.CountAsync();

    var users = await query
      .OrderByDescending(u => u.CreatedAt)
      .Skip((page - 1)*pageSize)
      .Take(pageSize)
      .Select(u => new UserResponseDto
      {
        Id = u.Id,
        Email = u.Email,
        CreatedAt = u.CreatedAt,
        Specialization = u.UserSpecializations
          .Select(us => us.Specialization)
          .ToList(),
        FirstName = u.UserData.FirstName,
        LastName = u.UserData.LastName,
        Skills = u.UserData.Skills,
        IsActive = u.IsActive
      })
      .ToListAsync();

    return new PagedResponse<UserResponseDto>
    {
      Page = page,
      PageSize = pageSize,
      TotalItems = totalItems,
      TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
      Items = users
    };
  }

  public async Task<UserResponseDto> GetUserByIdAsync(Guid userId, Guid currentUserId, bool canReadAll)  
  {
    var user = await _context.Users
      .AsNoTracking()
      .Where(u => u.Id == userId && u.IsActive)
      .Select(u => new UserResponseDto
      {
        Id = u.Id,
        Email = u.Email,
        Specialization = u.UserSpecializations
          .Select(us => us.Specialization)
          .ToList(),
        FirstName = u.UserData.FirstName,
        LastName = u.UserData.LastName,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
      })
      .FirstOrDefaultAsync();


    if (user == null || (!canReadAll && userId != currentUserId))
      throw new NotFoundAppException();

    return user;
  }

  public async Task<UserResponseDto> GetCurrentUserByIdAsync(Guid userId)
  {
    return await _context.Users
      .AsNoTracking()
      .Where(u => u.Id == userId && u.IsActive)
      .Select(u => new UserResponseDto
      {
        Id = u.Id,
        Email = u.Email,
        Specialization = u.UserSpecializations
          .Select(us => us.Specialization)
          .ToList(),
        FirstName = u.UserData.FirstName,
        LastName = u.UserData.LastName,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
      })
      .FirstOrDefaultAsync()
      ?? throw new NotFoundAppException();
  }
}