using backend.Domain.Entities.Enum;

namespace backend.Api.Controllers.Contracts.DTOs;

public class BaseContractDto
{
  public long ContractId { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public decimal Price { get; set; }
  // MAX BUDGET
  
  public int MaxRequests { get; set; }
  public ContractStatus ContractStatus { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public DateOnly Deadline { get; set; }
}