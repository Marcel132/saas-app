using System.ComponentModel.DataAnnotations.Schema;

public class RolesModel
{
  [Column("role_id")]
  public string RoleId { get; set; } = string.Empty;
  [Column("code")]
  public string Code { get; set; } = string.Empty;
  [Column("name")]
  public string Name { get; set; } = string.Empty;
  [Column("is_active")]
  public bool IsActive { get; set; } = true;
}