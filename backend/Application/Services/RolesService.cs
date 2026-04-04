using Microsoft.EntityFrameworkCore;

public class RoleService
{
  private readonly AppDbContext _context;
  public RoleService(
    AppDbContext context
  )
  {
    _context = context;
  }

  public async Task<HashSet<string>> GetEffectivePermissions(Guid userId)
  {
    var rolePermisions = await _context.UserRoles
      .Where(ur => ur.UserId == userId)
      .Join(_context.RolePermissions,
        ur => ur.RoleId,
        rp => rp.RoleId,
        (ur, rp) => rp.PermissionId
      )
      .Join(_context.Permissions,
        rp => rp,
        p => p.PermissionId,
        (rp, p) => new {p.Code, p.IsActive})
      .Where(p => p.IsActive)
      .Select(p => p.Code)
      .AsNoTracking()
      .ToListAsync();

    var effectivePermissions = new HashSet<string>(rolePermisions);

    var userPermissions = await _context.UserPermissions
      .Where(up => up.UserId == userId)
      .Join(_context.Permissions,
        up => up.PermissionId,
        p => p.PermissionId,
        (up, p) => new { up.IsDenied, p.Code }
      )
      .AsNoTracking()
      .ToListAsync();

    foreach(var up in userPermissions)
    {
      if(up.IsDenied)
        effectivePermissions.Remove(up.Code);
      else 
        effectivePermissions.Add(up.Code);
    }

    return effectivePermissions;
  }

}