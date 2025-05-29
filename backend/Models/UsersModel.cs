using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UsersModel
{
  [Column("uid")]
  [Required]
  public int Id { get; set; }

  [Column("email")]
  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Column("password_hash")]
  [Required]
  public string PasswordHash { get; set; } = string.Empty;

  public enum RoleType { Admin, Company, Pentester }
  [Column("user_role")]
  [Required]
  public RoleType Role { get; set; }

  [Column("is_active")]
  [Required]
  public bool IsActive { get; set; } = true;

  [Column("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


  public SessionModel? Session { get; set; }
  public UserDataModel? UserData { get; set; }

  public ICollection<OpinionModel>? Opinions { get; set; }
  public ICollection<ApiLogsModel>? ApiLogs { get; set; }
}