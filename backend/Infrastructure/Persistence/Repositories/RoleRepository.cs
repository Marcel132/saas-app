using Microsoft.EntityFrameworkCore;

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

  public async Task<IReadOnlyDictionary<string, Role>> GetByCodesAsync(
    IEnumerable<string> codes,
    CancellationToken ct = default)
  {
    var codeSet = codes.Distinct().ToArray();

    var roles = await _context.Roles
      .AsNoTracking()
      .Where(r => codeSet.Contains(r.Code) && r.IsActive)
      .ToListAsync(ct);

    if (roles.Count != codeSet.Length)
    {
      var missing = codeSet.Except(roles.Select(r => r.Code));
      throw new NotFoundAppException(); // * Create a custom exception for this case and include missing codes in the message for better debugging.
      // throw new NotFoundAppException(
      //   $"Missing roles: {string.Join(", ", missing)}"
      // );
    }

    return roles.ToDictionary(r => r.Code, r => r, StringComparer.OrdinalIgnoreCase);
  }
}