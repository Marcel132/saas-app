using backend.Api.Controllers.Reports.DTOs;

namespace backend.Domain.Interfaces.Repositories;

public interface IReportQueryRepository
{
  public Task<List<PentesterReportDto>> GetPentesterReportsAsync(Guid userId, CancellationToken ct);
}