namespace backend.Domain.Entities;

public class UserPermission
{
  public Guid UserId { get; private set; }
  public Guid PermissionId { get; private set; }
  public bool IsActive { get; private set; }
  public DateTime AssignedAt { get; private set; }

  private UserPermission() { } // EF

  public User User { get; private set; } = null!;
  public Permission Permission { get; private set; } = null!;

  public UserPermission(Guid userId, Guid permissionId)
  {
    ValidateRequiredFields(userId, permissionId);

    UserId = userId;
    PermissionId = permissionId;
    IsActive = true;
    AssignedAt = DateTime.UtcNow;
  }

  public void Deactivate()
  {
    if (!IsActive)
      throw new InvalidOperationAppException("Permission is deactivated");

    IsActive = false;
  }
  public void Activate()
  {
    if (IsActive)
      throw new InvalidOperationAppException("Permission is active");
    IsActive = true;
  }

  private static void ValidateRequiredFields(Guid userId, Guid permissionId)
  {
    if (userId == Guid.Empty)
      throw new BadRequestAppException("UserId is invalid.");
    if (permissionId == Guid.Empty)
      throw new BadRequestAppException("PermissionId is invalid.");
  }
}