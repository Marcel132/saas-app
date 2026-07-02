using backend.Domain.Entities.Enum;

namespace backend.Api.Controllers.Applications.DTOs;

public class ApplicationDto
{
  public long ApplicationId { get; set; }
  public long ContractId { get; set; }
  public string Title { get; set; } = null!;
  public decimal PricePerRequests { get; set; }
  public decimal MaxBudget { get; set; }
  public int MaxRequests { get; set; }
  public ContractApplicationStatus Status { get; set; }
  public DateTime AppliedAt { get; set; }
}