namespace backend.Domain.EntitiesNew;

public class RolePermission
{
  public Guid RoleId { get; private set; }
  public Guid PermissionId { get; private set; }

  private RolePermission() { } // EF

  public Role Role { get; private set; } = null!;
  public Permission Permission { get; private set; } = null!;


  public RolePermission(Guid roleId, Guid permissionId)
  {
    ValidateRequiredFields(roleId, permissionId);

    RoleId = roleId;
    PermissionId = permissionId;
  }

  private static void ValidateRequiredFields(Guid roleId, Guid permissionId)
  {
    if (roleId == Guid.Empty)
      throw new BadRequestAppException("RoleId is invalid.");
    if (permissionId == Guid.Empty)
      throw new BadRequestAppException("PermissionId is invalid.");
  }
}