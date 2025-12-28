using System.ComponentModel.DataAnnotations.Schema;

public class RolePermissionsModel
{
  [Column("role_id")]
  public Guid RoleId { get; set; }
  [Column("permission_id")]
  public Guid PermissionId { get; set; }
}