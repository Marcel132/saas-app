using backend.Api.Controllers.Reports.DTOs;
using backend.Domain.Interfaces.Repositories;
using backend.Domain.Interfaces.Services;

namespace backend.Application.Services;

public class ReportService : IReportService
{
  private readonly IReportQueryRepository _queryRepo;

  public ReportService(
    IReportQueryRepository queryRepo
  )
  {
    _queryRepo = queryRepo;
  }

  public async Task<List<PentesterReportDto>> GetPentesterReportsAsync(Guid userId, CancellationToken ct)
  {
    return await _queryRepo.GetPentesterReportsAsync(userId, ct);
  }
}