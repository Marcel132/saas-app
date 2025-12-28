using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("roles")]
public class RolesModel
{
  [Column("role_id")]
  [Key]
  public Guid RoleId { get; set; } = Guid.NewGuid();
  [Column("code")]
  public string Code { get; set; } = string.Empty;
  [Column("name")]
  public string Name { get; set; } = string.Empty;
  [Column("is_active")]
  public bool IsActive { get; set; } = true;
}