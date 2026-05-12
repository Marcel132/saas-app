public class RolePermission
{
  public Guid RoleId { get; private set; }
  public Guid PermissionId { get; private set; }

  //TODO: Add timestamp, isActive flag for soft delete

  RolePermission() {} //EF Core

  public RolePermission(Guid roleId, Guid permissionId)
  {
    ValidateRequiredFields(roleId, permissionId);
  
    RoleId = roleId;
    PermissionId = permissionId;
  }

  private static void ValidateRequiredFields(Guid roleId, Guid permissionId)
  {
    if(roleId == Guid.Empty)
      throw new ArgumentException("RoleId is invalid.");
    if(permissionId == Guid.Empty)
      throw new ArgumentException("PermissionId is invalid.");
  }
}