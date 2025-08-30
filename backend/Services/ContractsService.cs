using Microsoft.EntityFrameworkCore;

public class ContractsService
{
  private readonly AppDbContext _context;

  public ContractsService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<ContractModel>> GetAllContractsAsync()
  {
    try
    {
      return await _context.contracts.Include(c => c.Author)
        .Where(c => c.Author != null && DateTime.UtcNow <= (c.Deadline ?? DateTime.MaxValue))
        .ToListAsync();
    }
    catch (Exception ex)
    {
      throw new Exception("An error occurred while retrieving contracts.", ex);
    }
  }
  public async Task<ContractModel> GetContractByIdAsync(int contractId)
  {
    var contract = await _context.contracts
      .Include(c => c.Author)
      .FirstOrDefaultAsync(c => c.Id == contractId)
      ?? throw new KeyNotFoundException($"Contract with ID {contractId} not found.");

    return contract;
  }
}