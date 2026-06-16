using backend.Domain.Entities.Enum;

namespace backend.Api.Controllers.Me.DTOs;

public class ApplicationDto
{
  public long ApplicationId { get; set; }
  public long ContractId { get; set; }
  public string ContractTitle { get; set; } = null!;
  public decimal Price { get; set; }
  public ContractApplicationStatus ApplicationStatus { get; set; }
  public DateTime AppliedAt { get; set; }
}