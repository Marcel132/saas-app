using Microsoft.EntityFrameworkCore;
public class ContractRepository : IContractRepository
{
  private readonly AppDbContext _context;
  public ContractRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<Contract?> GetContractsByIdAsync(long contractId)
  {
    return await _context.Contracts
      .FirstOrDefaultAsync(c => c.ContractId == contractId);
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}