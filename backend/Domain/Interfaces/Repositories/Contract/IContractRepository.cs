public interface IContractRepository
{
  public Task<PagedResponse<ContractResponseDto>> GetContractsAsync(int page, int pageSize, string? search);
  public Task<ContractResponseDto?> GetContractsByIdAsync(long contractId);
}