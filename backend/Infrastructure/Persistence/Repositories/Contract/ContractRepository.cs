using Microsoft.EntityFrameworkCore;
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
      .FirstOrDefaultAsync(c => c.ContractId == contractId);
  }

  public async Task AddContractAsync(Contract contract)
  {
    _context.Contracts.Add(contract);
  }

  public async Task AddApplicationAsync(ContractApplication application)
  {
    _context.ContractApplications.Add(application);
  }
  public async Task<bool> HasAlreadyAppliedAsync(long contractId, Guid candidateId)
  {
    return await _context.ContractApplications
      .AnyAsync(a => a.ContractId == contractId && a.CandidateId == candidateId);
  }
}