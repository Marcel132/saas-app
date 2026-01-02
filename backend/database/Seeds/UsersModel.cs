using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class UsersModel
{
  // [Column("user_id")]
  public Guid Id { get; set; } = Guid.NewGuid();

  // [Column("email")]
  public string Email { get; set; } = string.Empty;

  // [Column("password_hash")]
  public string Password { get; set; } = string.Empty;

  // [Column("is_active")]
  public bool IsActive { get; set; } = true;

  // [Column("created_at")]
  public DateTime CreatedAt { get; set; }

  // [Column("specialization")]
  public List<string> SpecializationType { get; set; } = new List<string>();

  // [Column("failed_login_attempts")]
  public int FailedLoginAttempts { get; set; } = 0;

  // [Column("login_blocked_until")]
  public DateTime? LoginBlockedUntil { get; set; }

}