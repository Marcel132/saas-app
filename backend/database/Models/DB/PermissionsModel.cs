using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("permissions")]
public class PermissionsModel
{
  [Column("permission_id")]
  [Key]
  public Guid PermissionId { get; set; } = Guid.NewGuid();
  [Column("action")]
  public string Action { get; set; } = string.Empty;
  [Column("resource")]
  public string Resource { get; set; } = string.Empty;
  [Column("code")]
  public string Code { get; set; } = string.Empty;
  [Column("description")]
  public string Description { get; set; } = string.Empty;
  [Column("is_active")]
  public bool IsActive { get; set; }  
  [Column("created_at")]
  public DateTime CreatedAt { get; set; }
}