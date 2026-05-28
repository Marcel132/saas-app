public class ContractService
{
  private readonly IContractQueryRepository _contractQueryRepository;
  private readonly IContractRepository _contractRepository;
  public ContractService(IContractQueryRepository contractQueryRepository, IContractRepository contractRepository)
  {
    _contractQueryRepository = contractQueryRepository;
    _contractRepository = contractRepository;
  }

  public async Task<PagedResponse<ContractResponseDto>> GetContractsAsync(int page, int pageSize, string? search)
  {
    if(page <= 0 || pageSize <= 0)
      throw new ValueOutOfRangeAppException();
    if(!string.IsNullOrWhiteSpace(search) && search.Length > 100)
      throw new BadRequestAppException();

    var contracts = await _contractQueryRepository.GetContractsAsync(page, pageSize, search);
    return contracts;
  }

  public async Task<ContractResponseDto> GetContractByIdAsync(long contractId)
  {
    if(contractId <= 0)
      throw new ValueOutOfRangeAppException();
    
    var contract = await _contractQueryRepository.GetContractsByIdAsync(contractId)
      ?? throw new NotFoundAppException();
    
    return contract;
  }
  
  public async Task<ContractResponseDto> CreateContractAsync(Guid authorId, ContractRequestDto request)
  {
   var contract = new Contract(authorId, request.Title, request.Description, request.Price, request.Deadline);

   await _contractRepository.AddContractAsync(contract);
   await _contractRepository.SaveChangesAsync();

    return new ContractResponseDto
    {
      ContractId = contract.ContractId,
      AuthorId = contract.AuthorId,
      Title = contract.Title,
      Description = contract.Description,
      Price = contract.Price,
      ContractStatus = contract.ContractStatus,
      CreatedAt = contract.CreatedAt
    };
  }

  public async Task CloseContractAsync(Guid userId, long contractId)
  {
    if(userId == Guid.Empty)
      throw new BadRequestAppException();
    if(contractId <= 0)
      throw new ValueOutOfRangeAppException();

    var contract = await _contractRepository.GetContractByIdAsync(contractId)
      ?? throw new NotFoundAppException();

    if(contract.AuthorId != userId)
      throw new UnauthorizedAppException();
    
    contract.CancelContract();
    await _contractRepository.SaveChangesAsync();
  }

  public async Task UpdateContractAsync(Guid userId, long contractId, UpdateContractDto request)
  {
    if(userId == Guid.Empty)
      throw new BadRequestAppException();
    if(contractId <= 0)
      throw new ValueOutOfRangeAppException();

    var contract = await _contractRepository.GetContractByIdAsync(contractId)
      ?? throw new NotFoundAppException();

    if(contract.AuthorId != userId)
      throw new UnauthorizedAppException();

    if(!string.IsNullOrWhiteSpace(request.Title) || !string.IsNullOrWhiteSpace(request.Description))
      contract.UpdateContractDetails(request.Title, request.Description);

    if(request.Price.HasValue)
      contract.UpdatePrice(request.Price.Value);     

    if(request.NewDeadline.HasValue)
      contract.ExtendDeadline(request.NewDeadline.Value);

    await _contractRepository.SaveChangesAsync();
  }

  public async Task<List<ContractApplicationsDto>> GetContractApplicationsAsync(Guid userId, long contractId)
  {
    if(userId == Guid.Empty)
      throw new BadRequestAppException();
    if(contractId <= 0)
      throw new ValueOutOfRangeAppException();

    var contract = await _contractRepository.GetContractByIdAsync(contractId)
      ?? throw new NotFoundAppException();

    if(contract.AuthorId != userId)
      throw new UnauthorizedAppException();

    var applications = await _contractQueryRepository.GetContractApplicationsAsync(contractId);

    return applications;
  }

  public async Task ApplyToContractAsync(Guid candidateId, long contractId)
  {
    if(candidateId == Guid.Empty || contractId <= 0)
      throw new BadRequestAppException("Invalid candidate ID or contract ID.");

    var hasApplied = await _contractRepository.HasAlreadyAppliedAsync(contractId, candidateId);
    if(hasApplied)
      throw new InvalidOperationAppException("You have already applied to this contract.");

    var contract = await _contractRepository.GetContractByIdAsync(contractId)
      ?? throw new NotFoundAppException();

    if(contract.AuthorId == candidateId)
      throw new InvalidOperationAppException("You cannot apply to your own contract.");

    if(contract.IsExpired())
      throw new InvalidOperationAppException("Cannot apply to an expired contract.");
    
    if(contract.ContractStatus != ContractStatus.Open)
      throw new InvalidOperationAppException("Cannot apply to a non-open contract.");

    var application = new ContractApplication(contractId, candidateId);
    await _contractRepository.AddApplicationAsync(application);
    await _contractRepository.SaveChangesAsync();
  }
}