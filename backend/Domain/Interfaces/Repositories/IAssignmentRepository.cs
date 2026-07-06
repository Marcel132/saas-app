using backend.Domain.Entities;

namespace backend.Domain.Interfaces.Repositories;

public interface IAssignmentRepository
{
  public Task<ContractAssignment?> GetActiveAssignmentByContractIdAsync(long contractId);
  public Task AddAssignmentAsync(ContractAssignment assignment);
}