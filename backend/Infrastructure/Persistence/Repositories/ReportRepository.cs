using backend.Domain.Entities;
using backend.Domain.Interfaces.Repositories;

namespace backend.Infrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
  private readonly AppDbContext _context;
  public ReportRepository(
    AppDbContext appDbContext
  )
  {
    _context = appDbContext;
  }

  public async Task CreateReport(ContractReport report)
  {
    await _context.ContractReports.AddAsync(report);
  }
}