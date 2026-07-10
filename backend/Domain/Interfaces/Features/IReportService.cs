using backend.Api.Controllers.Reports.DTOs;

namespace backend.Domain.Interfaces.Features;

public interface IReportService
{
  public Task<List<PentesterReportDto>> GetPentesterReportsAsync(Guid userId, CancellationToken ct);
}