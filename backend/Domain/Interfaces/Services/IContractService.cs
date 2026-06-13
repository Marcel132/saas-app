public interface IContractService
{
  public Task<PagedResponse<ContractResponseDto>> GetContractsAsync(Guid userId, int page, int pageSize, string? search = null);
  public Task<ContractResponseDto> GetContractByIdAsync(long contractId, Guid userId);
  public Task<ContractResponseDto> CreateContractAsync(Guid authorId, ContractRequestDto request);
  public Task CloseContractAsync(Guid userId, long contractId);
  public Task UpdateContractAsync(Guid userId, long contractId, UpdateContractDto request);
  public Task<List<ContractApplicationsDto>> GetContractApplicationsAsync(Guid userId, long contractId);
  public Task ApplyToContractAsync(Guid candidateId, long contractId);
}