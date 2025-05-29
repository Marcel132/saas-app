using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SessionModel
{
  [Column("user_id")]
  [Required]
  [Key]
  public int UserId { get; set; }
  
  [ForeignKey("UserId")]
  public UsersModel User { get; set; } = null!;


  [Column("refresh_token")]
  [Required]
  public string RefreshToken { get; set; } = string.Empty;

  [Column("session_token")]
  [Required]
  public string SessionToken { get; set; } = string.Empty;

  [Column("created_at")]
  [Required]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}