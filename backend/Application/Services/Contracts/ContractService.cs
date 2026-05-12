public class ContractService
{
  private readonly IContractRepository _contractRepository;
  public ContractService(IContractRepository contractRepository)
  {
    _contractRepository = contractRepository;
  }

  public async Task<PagedResponse<ContractResponseDto>> GetContractsAsync(int page, int pageSize, string? search)
  {
    if(page <= 0 || pageSize <= 0)
      throw new ArgumentException("Page and pageSize must be greater than 0");
    if(!string.IsNullOrWhiteSpace(search) && search.Length > 100)
      throw new ArgumentException("Search query is too long");

    var contracts = await _contractRepository.GetContractsAsync(page, pageSize, search);
    return contracts;
  }
}