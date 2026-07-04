using backend.Domain.Entities.Enum;

namespace backend.Api.Controllers.Users.DTOs;

public class UserApplicationsDto
{
  public long ApplicationId { get; set; }
  public long ContractId { get; set; }
  // public Guid CompanyId { get; set; }
  public string Title { get; set; } = string.Empty;
  public decimal PricePerRequest{ get; set; }
  public decimal MaxBudget { get; set; }
  public int MaxRequests { get; set; }
  public ContractApplicationStatus Status { get; set; }
  public DateTime AppliedAt { get; set; }
}