using backend.Api.Controllers.Applications.DTOs;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Persistence.Repositories;

public class ApplicationQueryRepository : IApplicationQueryRepository
{
  private readonly AppDbContext _context;

  public ApplicationQueryRepository(
    AppDbContext appDbContext
  )
  {
    _context = appDbContext;
  }

  public async Task<List<ApplicationDto>> GetUserApplications(Guid userId)
  {
    var query = await _context.ContractApplications
      .AsNoTracking()
      .Where(ca => 
        ca.CandidateId == userId &&
        (
          ca.Status == ContractApplicationStatus.Accepted ||
          ca.Status == ContractApplicationStatus.Pending || 
          (
            ca.Status == ContractApplicationStatus.Rejected &&
            ca.Contract.Deadline > DateTime.UtcNow &&
            ca.Contract.ContractStatus == ContractStatus.Open
          )
        )
      )

      .Select(x => new ApplicationDto
      {
        ApplicationId = x.ApplicationId,
        ApplicationStatus = x.Status,
        AppliedAt = x.AppliedAt,
        ContractId = x.ContractId,
        ContractTitle = x.Contract.Title,
        Price = x.Contract.Price
      })
      .ToListAsync();
  
    return query;
  }
}