namespace backend.Domain.Entities;

public class Role
{
  public Guid RoleId { get; private set; }
  public string Code { get; private set; } = string.Empty;
  public string Name { get; private set; } = string.Empty;
  public bool IsActive { get; private set; }

  private readonly List<RolePermission> _rolePermissions = new();
  public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();
  private Role() { }
}

