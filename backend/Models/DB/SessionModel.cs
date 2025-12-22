using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SessionModel
{
  [Key]
  [Column("id")]
  public int Id { get; set; }
  [Column("user_id")]
  [Required]
  public int UserId { get; set; }

  [ForeignKey("UserId")]
  public UsersModel User { get; set; } = null!;


  [Column("refresh_token")]
  [Required]
  public string RefreshToken { get; set; } = string.Empty;

  [Column("auth_token")]
  [Required]
  public string AuthToken { get; set; } = string.Empty;

  [Column("created_at")]
  public DateTime? CreatedAt { get; set; }

  [Column("expires_at")]
  public DateTime? ExpiresAt { get; set; }

  [Column("user_agent")]
  [Required]
  public string UserAgent { get; set; } = string.Empty;

  [Column("ip")]
  [Required]
  public string Ip { get; set; } = string.Empty;

  [Column("revoked")]
  [Required]
  public bool Revoked { get; set; } = false;
}