using backend.Api.Controllers.Reports.DTOs;
using backend.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Persistence.Repositories;

public class ReportQueryRepository : IReportQueryRepository
{
  private readonly AppDbContext _context;

  public ReportQueryRepository(
    AppDbContext appDbContext
  )
  {
    _context = appDbContext;
  }

  public async Task<List<PentesterReportDto>> GetPentesterReportsAsync(Guid userId, CancellationToken ct)
  {
    var query = _context.ContractReports
      .AsNoTracking()
      .Where(cr =>
        cr.ContractAssignment.PentesterId == userId &&
        cr.ContractAssignment.IsActive
      ).Select(cr => new PentesterReportDto
      {
        ReportId = cr.Id,
        AssignmentId = cr.AssignmentId,
        ContractId = cr.ContractAssignment.ContractId,
        CreatedAt = cr.CreatedAt,
        ReportStatus = cr.Status,
        Title = cr.ContractAssignment.Contract.Title,
        Requests = _context.ContractRequests
          .Where(r => r.AssignmentId == cr.AssignmentId && r.IsActive)
          .Select(r => new ReportRequestDto
          {
            RequestId = r.Id,
            Title = r.Title,
            Status = r.Status
          })
          .ToList()
      }).ToListAsync(ct);

    
    return await query;
  }
}