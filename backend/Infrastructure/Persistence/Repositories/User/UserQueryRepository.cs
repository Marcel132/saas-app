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

      var escaped = search.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
      query = query
        .Where(u =>
        EF.Functions.ILike(u.Email, $"%{escaped}%", @"\") ||
        EF.Functions.ILike(u.UserData.FirstName, $"%{escaped}%", @"\") ||
        EF.Functions.ILike(u.UserData.LastName, $"%{escaped}%", @"\") ||
        EF.Functions.ILike(u.UserData.Nickname ?? "", $"%{escaped}%", @"\")
      );
    }

    var totalItems = await query.CountAsync();

    var users = await query
      .OrderByDescending(u => u.CreatedAt)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .Select(u => new UserResponsePublicDto
      {
        Nickname = u.UserData.Nickname ?? string.Empty,
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
        Nickname = u.UserData.Nickname ?? string.Empty,
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
    var query = _context.Users
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
        Nickname = u.UserData.Nickname ?? string.Empty,
        Skills = u.UserData.Skills,
        CompanyName = u.UserData.CompanyName,
        CompanyNip = u.UserData.CompanyNip,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt,
        Roles = u.UserRoles
          .Join(_context.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r.Name)
          .ToHashSet(),
        Permissions = u.UserRoles
          .Join(_context.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r)
          .SelectMany(r => r.RolePermissions)
          .Join(_context.Permissions, rp => rp.PermissionId, p => p.PermissionId, (rp, p) => p.Code)
          .ToHashSet()
      });

      return await query.FirstOrDefaultAsync() ?? throw new NotFoundAppException();
  }

  public async Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status = null)
  {
    var query = _context.Contracts
      .AsNoTracking()
      .Where(c => 
      c.AuthorId == userId ||
      _context.ContractAssignments.Any(ca => 
      ca.ContractId == c.ContractId 
      && ca.DeveloperId == userId
      ));
    
    if(status.HasValue)
      query = query.Where(c => c.ContractStatus == status.Value);
    
    return await query
      .Select(c => new UserContractsDto
      {
        ContractId = c.ContractId,
        Title = c.Title,
        Description = c.Description,
        ContractStatus = c.ContractStatus,
        CreatedAt = c.CreatedAt
      })
      .ToListAsync();
  }
  public async Task<List<UserApplicationsDto>> GetApplicationsAsync(Guid userId, ContractApplicationStatus? status)
  {
    var query = _context.ContractApplications
      .AsNoTracking()
      .Where(ca => ca.CandidateId == userId)
      .Join(
        _context.Contracts,
        ca => ca.ContractId,
        c => c.ContractId,
        (ca, c) => new UserApplicationsDto
        {
          ApplicationId = ca.ApplicationId,
          ContractId = ca.ContractId,
          CompanyId = c.AuthorId,
          Status = ca.Status,
          AppliedAt = ca.AppliedAt
        }
      );

    if(status.HasValue)
      query = query.Where(ca => ca.Status == status.Value);
    
    return await query.ToListAsync();
  } 
}
