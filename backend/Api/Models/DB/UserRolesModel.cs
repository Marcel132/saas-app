using System.ComponentModel.DataAnnotations.Schema;

[Table("user_roles")]
public class UserRolesModel
{
  [Column("user_id")]
  public Guid UserId { get; set; }
  [Column("role_id")]
  public Guid RoleId { get; set; }
  [Column("assigned_at")]
  public DateTime AssignedAt { get; set; }
}