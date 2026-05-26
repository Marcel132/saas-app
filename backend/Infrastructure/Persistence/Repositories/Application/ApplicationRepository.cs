using Microsoft.EntityFrameworkCore;
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
      .FirstOrDefaultAsync(ca => ca.ApplicationId == applicationId);
  }

  public async Task<List<ContractApplication>> GetApplicationsByContractIdAsync(long contractId, Guid? excludeCandidateId = null)
  {
    var query = _context.ContractApplications.Where(ca => ca.ContractId == contractId);

    if (excludeCandidateId.HasValue)
    {
      query = query.Where(ca => ca.CandidateId != excludeCandidateId.Value);
    }

    return await query.ToListAsync();
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}