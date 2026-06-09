using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

  public class MeQueryRepository : IMeQueryRepository
  {
    private readonly AppDbContext _appDbContext;

    public MeQueryRepository(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }

    public async Task<List<ApplicationDto>> GetApplications(Guid userId)
    {

      return await _appDbContext.ContractApplications
        .AsNoTracking()
        .Join(_appDbContext.Contracts,
        ca => ca.ContractId,
        c => c.ContractId,
        (ca, c) => new {Application = ca, Contract = c}
        )
        .Where(x => 
          x.Application.CandidateId == userId && 
          (
            x.Application.Status == ContractApplicationStatus.Pending ||
            x.Application.Status == ContractApplicationStatus.Accepted ||
            (
              x.Application.Status == ContractApplicationStatus.Rejected && x.Contract.Deadline > DateTime.UtcNow
            )
          )
        )
        .Select(x => new ApplicationDto
        {
          ApplicationId = x.Application.ApplicationId,
          ContractId = x.Application.ContractId,
          ContractTitle = x.Contract.Title,
          Price = x.Contract.Price,
          AppliedAt = x.Application.AppliedAt,
          ApplicationStatus = x.Application.Status
        })
        .ToListAsync();
    }
  }