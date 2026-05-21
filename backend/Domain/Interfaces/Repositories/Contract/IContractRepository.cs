public interface IContractRepository
{
  public Task<Contract?> GetContractsByIdAsync(long contractId);
  public Task SaveChangesAsync();
}