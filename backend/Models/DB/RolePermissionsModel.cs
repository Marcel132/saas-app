using System.ComponentModel.DataAnnotations.Schema;

public class RolePermissionsModel
{
  [Column("role_id")]
  public string RoleId { get; set; } = string.Empty;
  [Column("permission_id")]
  public Guid PermissionId { get; set; }
}