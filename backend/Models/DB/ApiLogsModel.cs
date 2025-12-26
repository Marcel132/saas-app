using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ApiLogsModel
{
  [Column("id")]
  [Required]
  [Key]
  public int Id { get; set; }

  [Column("user_id")]
  [Required]
  public Guid UserId { get; set; }

  [ForeignKey("UserId")] 
  public UsersModel User { get; set; } = null!;

  [Column("route")]
  [Required]
  public string Route { get; set; } = string.Empty;

  [Column("status_code")]
  [Required]
  public int StatusCode { get; set; }

  [Column("status_message")]
  [Required]
  public string StatusMessage { get; set; } = string.Empty;

  [Column("timestamp")]
  [Required]
  public DateTime? Timestamp { get; set; } = DateTime.UtcNow;

  [Column("method")]
  [Required]
  public string Method { get; set; } = string.Empty;

  [Column("ip_address")]
  [Required]
  public string IpAddress { get; set; } = string.Empty;
}