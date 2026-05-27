public interface IAssignmentRepository
{
  public Task<ContractAssignment?> GetActiveAssignmentByContractIdAsync(long contractId);
  public Task AddAssignmentAsync(ContractAssignment assignment);
  public Task SaveChangesAsync();
}