using System.ComponentModel.DataAnnotations.Schema;

[Table("role_permissions")]
public class RolePermissionsModel
{
  [Column("role_id")]
  public Guid RoleId { get; set; }
  [Column("permission_id")]
  public Guid PermissionId { get; set; }
}