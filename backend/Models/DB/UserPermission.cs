using System.ComponentModel.DataAnnotations.Schema;

public class UserPermissionModel
{
  [Column("user_id")]
  public int UserId { get; set; }
  [Column("permission_id")]
  public Guid PermissionId { get; set; }
  [Column("is_denied")]
  public bool IsDenied { get; set; }
  [Column("granted_at")]
  public DateTime GrantedAt { get; set; }
}