public interface IContractQueryRepository
{
  public Task<PagedResponse<ContractResponseDto>> GetContractsAsync(Guid userId, int page, int pageSize, string? search);
  public Task<ContractResponseDto?> GetContractsByIdAsync(long contractId, Guid userId);
  public Task<List<ContractApplicationsDto>> GetContractApplicationsAsync(long contractId);

}