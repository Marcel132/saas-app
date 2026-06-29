using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Persistence.Repositories;

public class ApplicationRepository : IApplicationRepository
{
  private readonly AppDbContext _context;
  public ApplicationRepository(AppDbContext dbContext)
  {
    _context = dbContext;
  }

  public async Task<ContractApplication?> GetApplicationAsync(long applicationId)
  {
    return await _context.ContractApplications
      .Include(ca => ca.Contract)
      .FirstOrDefaultAsync(ca => ca.Id == applicationId);
  }

  public async Task<List<ContractApplication>> GetApplicationsByContractIdAsync(long contractId, Guid? excludeCandidateId = null)
  {
    var query = _context.ContractApplications
      .Where(ca =>
        ca.ContractId == contractId &&
        ca.Status == ContractApplicationStatus.Pending
      );

    if (excludeCandidateId.HasValue)
    {
      query = query.Where(ca => ca.UserId != excludeCandidateId.Value);
    }

    return await query.ToListAsync();
  }
}