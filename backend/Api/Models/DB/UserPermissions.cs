using System.ComponentModel.DataAnnotations.Schema;

public class UserPermissionsModel
{
  [Column("user_id")]
  public Guid UserId { get; set; }
  [Column("permission_id")]
  public Guid PermissionId { get; set; }
  [Column("is_denied")]
  public bool IsDenied { get; set; }
  [Column("granted_at")]
  public DateTime GrantedAt { get; set; }
}