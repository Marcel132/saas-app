using Microsoft.EntityFrameworkCore;
public class AssignmentRepository : IAssignmentRepository
{
  private readonly AppDbContext _context;
  public AssignmentRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<ContractAssignment?> GetActiveAssignmentByContractIdAsync(long contractId)
  {
    return await _context.ContractAssignments
      .Where(ca => ca.ContractId == contractId && ca.IsActive)
      .FirstOrDefaultAsync();
  }
  public async Task AddAssignmentAsync(ContractAssignment assignment)
  {
    await _context.ContractAssignments.AddAsync(assignment);
  }
  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}