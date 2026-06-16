using backend.Domain.Entities;
using backend.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
  private readonly AppDbContext _context;

  public RoleRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<Role> GetByCodeAsync(string code, CancellationToken ct = default)
  {
    var role = await _context.Roles
      .AsNoTracking()
      .FirstOrDefaultAsync(r => r.Code == code && r.IsActive, ct)
      ?? throw new InvalidOperationException($"Role '{code}' not found");

    return role;
  }

  public async Task<IReadOnlyDictionary<string, Role>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken ct = default)
  {
    var codeSet = codes.Distinct().ToArray();

    var roles = await _context.Roles
      .AsNoTracking()
      .Where(r => codeSet.Contains(r.Code) && r.IsActive)
      .ToListAsync(ct);

    if (roles.Count != codeSet.Length)
    {
      var missing = codeSet.Except(roles.Select(r => r.Code));
      throw new NotFoundAppException(); // TODO: Create a custom exception for this case and include missing codes in the message for better debugging.
    }

    return roles.ToDictionary(r => r.Code, r => r, StringComparer.OrdinalIgnoreCase);
  }

  public async Task<HashSet<string>> GetEffectivePermissionsAsync(Guid userId)
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
        (rp, p) => new { p.Code, p.IsActive })
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

    foreach (var up in userPermissions)
    {
      if (up.IsDenied)
        effectivePermissions.Remove(up.Code);
      else
        effectivePermissions.Add(up.Code);
    }

    return effectivePermissions;
  }
}