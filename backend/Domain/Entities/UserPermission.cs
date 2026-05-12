public class UserPermission
{
  public Guid UserId { get; private set; }
  public Guid PermissionId { get; private set; }
  public bool IsDenied { get; private set; } = false;
  public DateTime AssignedAt { get; private set; }

  private UserPermission() {} //EF Core

  public UserPermission(Guid userId, Guid permissionId, bool isDenied)
  {
    ValidateRequiredFields(userId, permissionId);
  
    UserId = userId;
    PermissionId = permissionId;
    IsDenied = isDenied;
    AssignedAt = DateTime.UtcNow;
  }
  private static void ValidateRequiredFields(Guid userId, Guid permissionId)
  {
    if(userId == Guid.Empty)
      throw new ArgumentException("UserId is invalid.");
    if(permissionId == Guid.Empty)
      throw new ArgumentException("PermissionId is invalid.");
  }
}