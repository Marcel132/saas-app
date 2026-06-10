public interface IMeQueryRepository
{
  public Task<List<ApplicationDto>> GetApplicationsAsync(Guid userId);
  public Task<PagedResponse<ContractResponseDto>> GetContractsAsync(Guid userId, int page, int pageSize, string? search);
  public Task<ContractResponseDto?> GetContractsByIdAsync(long contractId, Guid userId);

}