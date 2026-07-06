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
}