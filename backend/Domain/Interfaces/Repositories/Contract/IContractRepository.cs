public interface IContractRepository
{
  public Task<Contract?> GetContractsByIdAsync(long contractId);
  public Task AddContractAsync(Contract contract);
  public Task AddApplicationAsync(ContractApplication application);
  public Task SaveChangesAsync();
}