using backend.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using backend.Domain.Entities;

namespace backend.Infrastructure.Persistence.Repositories;

public class ContractRepository : IContractRepository
{
  private readonly AppDbContext _context;
  public ContractRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<Contract?> GetContractByIdAsync(long contractId)
  {
    return await _context.Contracts
      .FirstOrDefaultAsync(c => c.Id == contractId);
  }

  public async Task AddContractAsync(Contract contract)
  {
    await _context.Contracts.AddAsync(contract);
  }

  public async Task AddApplicationAsync(ContractApplication application)
  {
    await _context.ContractApplications.AddAsync(application);
  }
  public async Task<bool> HasAlreadyAppliedAsync(long contractId, Guid candidateId)
  {
    return await _context.ContractApplications
      .AnyAsync(a => a.ContractId == contractId && a.UserId == candidateId);
  }
}