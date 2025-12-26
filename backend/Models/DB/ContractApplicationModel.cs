using System.ComponentModel.DataAnnotations.Schema;

public class ContractApplicationModel
{
  [Column("id")]
  public int Id { get; set; }
  [Column("contract_id")]
  public Guid ContractId { get; set; }
  [Column("user_id")]
  public Guid UserId { get; set; } 
  [Column("applied_at")]
  public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

}