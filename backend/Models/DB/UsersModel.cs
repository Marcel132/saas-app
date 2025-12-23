using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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

  [Column("is_active")]
  [Required]
  public bool IsActive { get; set; } = true;

  [Column("created_at")]
  public DateTime CreatedAt { get; set; }

  [Column("specialization")]
  public List<string> SpecializationType { get; set; } = new List<string>();
  // public string SpecializationType { get; set; } = string.Empty;

  [Column("skills")]
  public string Skills { get; set; } = string.Empty;

  [Column("failed_login_attempts")]
  public int FailedLoginAttempts { get; set; } = 0;

  [Column("login_blocked_until")]
  public DateTime? LoginBlockedUntil { get; set; }

  // public ICollection<SessionModel> Sessions { get; set; } = new List<SessionModel>();
  // public UserDataModel UserData { get; set; } = null!;

  // public ICollection<OpinionModel>? Opinions { get; set; }
  // public ICollection<ApiLogsModel>? ApiLogs { get; set; }
  // public ICollection<ContractModel> Contracts { get; set; } = new List<ContractModel>();
  // public ICollection<ContractApplicationModel> ContractsApplication { get; set; } = new List<ContractApplicationModel>();
}