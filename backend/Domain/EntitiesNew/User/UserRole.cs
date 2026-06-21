namespace backend.Domain.EntitiesNew;

public class UserRole
{
  public Guid UserId { get; private set; }
  public Guid RoleId { get; private set; }
  public DateTime AssignedAt { get; private set; }

  private UserRole() { } // EF
  
  public User User { get; private set; } = null!;
  public Role Role { get; private set; } = null!;
  
  public UserRole(Guid userId, Guid roleId)
  {
    ValidateRequiredFields(userId, roleId);

    UserId = userId;
    RoleId = roleId;
    AssignedAt = DateTime.UtcNow;
  }

  private static void ValidateRequiredFields(Guid userId, Guid roleId)
  {
    if (userId == Guid.Empty)
      throw new BadRequestAppException("User ID jest puste");
    if (roleId == Guid.Empty)
      throw new BadRequestAppException("Role ID jest puste");
  }
}