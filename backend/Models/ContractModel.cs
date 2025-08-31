using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ContractModel
{
  [Column("id")]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  [Key]
  public int Id { get; set; }

  [Column("author_id")]
  [Required]
  public int AuthorId { get; set; }

  [Column("price")]
  [Required]
  public decimal Price { get; set; }

  [Column("status")]
  [Required]
  public StatusOfContractModel Status { get; set; } = StatusOfContractModel.Pending;

  [Column("description")]
  [MaxLength(500)]
  [Required]
  public string Description { get; set; } = string.Empty;

  [Column("created_at")]
  public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("updated_at")]
  public DateTime? UpdatedAt { get; set; }

  [Column("deadline")]
  public DateTime? Deadline { get; set; }

  [ForeignKey("AuthorId")]
  public UsersModel? Author { get; set; }
}