using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("contract_applications")]
public class ContractApplicationModel
{
  [Column("id")]
  [Key]
  public int Id { get; set; }
  [Column("contract_id")]
  public Guid ContractId { get; set; }
  [Column("user_id")]
  public Guid UserId { get; set; } 
  [Column("applied_at")]
  public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

}