public interface IApplicationRepository
{
  public Task<ContractApplication?> GetApplicationAsync(long applicationId);
  public Task<List<ContractApplication>> GetApplicationsByContractIdAsync(long contractId, Guid? excludeCandidateId = null);
}