using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UsersModel
{
  [Column("uid")]
  public int Id { get; set; }

  [Column("email")]
  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;

  [Column("password_hash")]
  [Required]
  public string Password { get; set; } = string.Empty;

  [Column("user_role")]
  [Required]
  public RoleEnum Role { get; set; }

  [Column("is_active")]
  [Required]
  public bool IsActive { get; set; } = true;

  [Column("created_at")]
  public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("specialization")]
  public SpecializationEnum SpecializationType { get; set; } = SpecializationEnum.Pentester;

  [Column("skills")]
  public string Skills { get; set; } = string.Empty;


  public ICollection<SessionModel> Sessions { get; set; } = new List<SessionModel>();
  public UserDataModel UserData { get; set; } = null!;

  public ICollection<OpinionModel>? Opinions { get; set; }
  public ICollection<ApiLogsModel>? ApiLogs { get; set; }
  public ICollection<ContractModel> Contracts { get; set; } = new List<ContractModel>();
}