using backend.Domain.Entities.Enum;

namespace backend.Api.Controllers.Reports.DTOs;

public class ReportRequestDto
{
  public long RequestId { get; set; }

  public string Title { get; set; } = string.Empty;

  public ContractRequestStatus Status { get; set; }
}