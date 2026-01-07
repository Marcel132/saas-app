using Microsoft.EntityFrameworkCore;

public class RoleRepository : IRoleRepository
{
   private readonly AppDbContext _db;

  public RoleRepository(AppDbContext db)
  {
    _db = db;
  }

  public async Task<Role> GetByCodeAsync(string code, CancellationToken ct = default)
  {
    var role = await _db.Roles
      .AsNoTracking()
      .FirstOrDefaultAsync(r => r.Code == code && r.IsActive, ct);

    if (role == null)
      throw new InvalidOperationException($"Role '{code}' not found");

    return role;
  }

  public async Task<IReadOnlyDictionary<string, Role>> GetByCodesAsync(
    IEnumerable<string> codes,
    CancellationToken ct = default)
  {
    var codeSet = codes.Distinct().ToArray();

    var roles = await _db.Roles
      .AsNoTracking()
      .Where(r => codeSet.Contains(r.Code) && r.IsActive)
      .ToListAsync(ct);

    if (roles.Count != codeSet.Length)
    {
      var missing = codeSet.Except(roles.Select(r => r.Code));
      throw new InvalidOperationException(
        $"Missing roles: {string.Join(", ", missing)}"
      );
    }

    return roles.ToDictionary(r => r.Code);
  }
}