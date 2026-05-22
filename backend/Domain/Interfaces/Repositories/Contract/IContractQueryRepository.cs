public interface IContractQueryRepository
{
  public Task<PagedResponse<ContractResponseDto>> GetContractsAsync(int page, int pageSize, string? search);
  public Task<ContractResponseDto?> GetContractsByIdAsync(long contractId);
  public Task<List<ContractApplicationsDto>> GetContractApplicationsAsync(long contractId);
}