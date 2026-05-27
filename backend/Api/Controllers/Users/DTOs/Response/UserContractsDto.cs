public class UserContractsDto
{
  public long ContractId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public decimal Price { get; set; }
  public ContractStatus ContractStatus { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime Deadline { get; set; }
}