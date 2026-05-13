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
      throw new ValueOutOfRangeAppException();
    if(!string.IsNullOrWhiteSpace(search) && search.Length > 100)
      throw new BadRequestAppException();

    var contracts = await _contractRepository.GetContractsAsync(page, pageSize, search);
    return contracts;
  }

  public async Task<ContractResponseDto> GetContractByIdAsync(long contractId)
  {
    if(contractId <= 0)
      throw new ValueOutOfRangeAppException();
    
    var contract = await _contractRepository.GetContractsByIdAsync(contractId)
      ?? throw new NotFoundAppException();
    
    return contract;
  }
}