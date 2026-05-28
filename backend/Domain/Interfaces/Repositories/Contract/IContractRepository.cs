public interface IContractRepository
{
  public Task<Contract?> GetContractByIdAsync(long contractId);
  public Task AddContractAsync(Contract contract);
  public Task AddApplicationAsync(ContractApplication application);
  public Task<bool> HasAlreadyAppliedAsync(long contractId, Guid candidateId);
}