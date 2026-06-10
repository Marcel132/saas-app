public class MeService
{
  private readonly IMeQueryRepository _meQueryRepository;
  private readonly IMeRepository _meRepository;
  private readonly IUnitOfWork _unitOfWork;
  public MeService(
    IMeQueryRepository meQueryRepository,
    IMeRepository meRepository,
    IUnitOfWork unitOfWork
  )
  {
    _meQueryRepository = meQueryRepository;
    _meRepository = meRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<List<ApplicationDto>> GetApplications(Guid userId)
  {
    return await _meQueryRepository.GetApplicationsAsync(userId);
  }

  public async Task<PagedResponse<ContractResponseDto>> GetContracts(Guid userId, int page, int pageSize, string? search)
  {
    return await _meQueryRepository.GetContractsAsync(userId, page, pageSize, search);
  }
}