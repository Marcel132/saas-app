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
        ca.UserId == userId &&
        (
          ca.Status == ContractApplicationStatus.Accepted ||
          ca.Status == ContractApplicationStatus.Pending ||
          (
            ca.Status == ContractApplicationStatus.Rejected &&
            ca.Contract.RecruitmentDeadline > DateOnly.FromDateTime(DateTime.UtcNow) &&
            ca.Contract.Status == ContractStatus.Open
          )
        )
      )
      .Select(x => new ApplicationDto
      {
        ApplicationId = x.Id,
        ContractId = x.ContractId,
        Title = x.Contract.Title,
        PricePerRequests = x.Contract.PricePerRequest,
        MaxBudget = x.Contract.MaxBudget,
        Status = x.Status,
        AppliedAt = x.AppliedAt,
      })
      .ToListAsync();

    return query;
  }
}