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
    await _context.SaveChangesAsync();
  }

  public async Task AddApplicationAsync(ContractApplication application)
  {
    _context.ContractApplications.Add(application);
    await _context.SaveChangesAsync();
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}