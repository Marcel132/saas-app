using backend.Domain.Entities;
using backend.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Persistence.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
  private readonly AppDbContext _context;
  public AssignmentRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<ContractAssignment?> GetActiveAssignmentByContractIdAsync(long contractId, CancellationToken ct)
  {
    return await _context.ContractAssignments
      .Where(ca => ca.ContractId == contractId && ca.IsActive)
      .FirstOrDefaultAsync(ct);
  }
  public async Task AddAssignmentAsync(ContractAssignment assignment)
  {
    await _context.ContractAssignments.AddAsync(assignment);
  }
}