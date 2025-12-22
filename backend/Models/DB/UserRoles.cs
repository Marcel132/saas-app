using System.ComponentModel.DataAnnotations.Schema;

public class UserRolesModel
{
  [Column("user_id")]
  public int UserId { get; set; }
  [Column("role_id")]
  public string RoleId { get; set; } = string.Empty;
  [Column("assigned_at")]
  public DateTime AssignedAt { get; set; }
}