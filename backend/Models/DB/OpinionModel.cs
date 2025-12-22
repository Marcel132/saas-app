using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class OpinionModel
{
  [Column("id")]
  [Required]
  [Key]
  public int Id { get; set; }

  public UsersModel User { get; set; } = null!;

  [Column("author_id")]
  [Required]
  public int AuthorId { get; set; }

  [Column("target_id")]
  [Required]
  public int TargetId { get; set; }

  [Column("rating_accuracy")]
  [Required]
  [Range(1, 5, ErrorMessage = "Rating accuracy must be between 1 and 5.")]
  public int RatingAccuracy { get; set; }

  [Column("rating_quality")]
  [Required]
  [Range(1, 5, ErrorMessage = "Rating quality must be between 1 and 5.")]
  public int RatingQuality { get; set; }

  [Column("rating_time")]
  [Required]
  [Range(1, 5, ErrorMessage = "Rating time must be between 1 and 5.")]
  public int RatingTime { get; set; }

  [Column("description")]
  [Required]
  public string Description { get; set; } = string.Empty;

  [Column("created_at")]
  public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}