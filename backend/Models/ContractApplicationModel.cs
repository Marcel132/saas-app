using System.ComponentModel.DataAnnotations.Schema;

public class ContractApplicationModel
{
  [Column("id")]
  public int Id { get; set; }
  [Column("contract_id")]
  public int ContractId { get; set; }
  [Column("user_id")]
  public int UserId { get; set; } 
  [Column("applied_at")]
  public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

  // nawigacja
  public ContractModel? Contract { get; set; }
  public UsersModel User { get; set; } = null!;
}