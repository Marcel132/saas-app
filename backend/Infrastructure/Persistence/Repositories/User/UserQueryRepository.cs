using Microsoft.EntityFrameworkCore;

public class UserQueryRepository : IUserQueryRepository
{
  private readonly AppDbContext _context;
  public UserQueryRepository
  (
    AppDbContext context
  )
  {
    _context = context;
  }

  public async Task<PagedResponse<UserResponsePublicDto>> GetAllAsync(int page, int pageSize, string? search = null)
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
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .Select(u => new UserResponsePublicDto
      {
        Nickname = u.UserData.Nickname,
        Skills = u.UserData.Skills,
        CompanyName = u.UserData.CompanyName ?? string.Empty,
        CreatedAt = u.CreatedAt,
        Specialization = u.UserSpecializations
          .Select(us => us.Specialization)
          .ToList(),
      })
      .ToListAsync();

    return new PagedResponse<UserResponsePublicDto>
    {
      Page = page,
      PageSize = pageSize,
      TotalItems = totalItems,
      TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
      Items = users
    };
  }
  public async Task<UserResponsePublicDto> GetUserByIdAsync(Guid userId)
  {
    return await _context.Users
      .AsNoTracking()
      .Where(u => u.Id == userId && u.IsActive)
      .Select(u => new UserResponsePublicDto
      {
        Nickname = u.UserData.Nickname,
        Skills = u.UserData.Skills,
        Specialization = u.UserSpecializations
          .Select(us => us.Specialization)
          .ToList(),
        CompanyName = u.UserData.CompanyName ?? string.Empty,
        CreatedAt = u.CreatedAt
      })
      .FirstOrDefaultAsync()
      ?? throw new NotFoundAppException();
  }

  public async Task<UserResponsePrivateDto> GetCurrentUserByIdAsync(Guid userId)
  {
    // users -> user_roles -> role.name

    var userRole = await _context.UserRoles
      .Where(ur => ur.UserId == userId)
      .Join(_context.Roles,
        ur => ur.RoleId,
        r => r.RoleId,
        (ur, r) => r.Name)
      .ToListAsync();
    
    var userPermissions = await _context.UserPermissions
      .Where(up => up.UserId == userId)
      .Join(_context.Permissions,
        up => up.PermissionId,
        p => p.PermissionId,
        (up, p) => p.Code)
      .ToListAsync();
    
    return await _context.Users
      .AsNoTracking()
      .Where(u => u.Id == userId && u.IsActive)
      .Select(u => new UserResponsePrivateDto
      {
        Id = u.Id,
        Email = u.Email,
        Specialization = u.UserSpecializations
          .Select(us => us.Specialization)
          .ToList(),
        FirstName = u.UserData.FirstName,
        LastName = u.UserData.LastName,
        Skills = u.UserData.Skills,
        CompanyName = u.UserData.CompanyName ?? string.Empty,
        CompanyNip = u.UserData.CompanyNip ?? string.Empty,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt,
        Roles = userRole.ToHashSet(),
        Permissions = userPermissions.ToHashSet()
      })
      .FirstOrDefaultAsync()
      ?? throw new NotFoundAppException();
  }
}
