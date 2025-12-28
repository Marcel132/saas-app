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
    var roleIds = await _context.UserRoles
      .Where(ur => ur.UserId == userId)
      .Select(ur => ur.RoleId)
      .ToListAsync();

    var rolePermissionIds = await _context.RolePermissions
      .Where(rp => roleIds.Contains(rp.RoleId))
      .Select(rp => rp.PermissionId)
      .ToListAsync();

    var permissionCodes = await _context.Permissions
      .Where(pc => rolePermissionIds.Contains(pc.PermissionId) && pc.IsActive)
      .Select(pc => pc.Code)
      .ToListAsync();

    var effectivePermissions = new HashSet<string>(permissionCodes);

    var userPermissions = await _context.UserPermissions
      .Where(up => up.UserId == userId)
      .Select(up => new
      {
        up.IsDenied,
        PermisssonCode = _context.Permissions
          .Where(p => p.PermissionId == up.PermissionId)
          .Select(p => p.Code)
          .First()
      })
      .ToListAsync();

    foreach(var up in userPermissions)
    {
      if(up.IsDenied)
        effectivePermissions.Remove(up.PermisssonCode);
      else 
        effectivePermissions.Add(up.PermisssonCode);
    }

    return effectivePermissions;
  }


}