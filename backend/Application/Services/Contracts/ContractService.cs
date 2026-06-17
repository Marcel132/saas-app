using backend.Api.Controllers;
using backend.Api.Controllers.Contracts.DTOs;
using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;
using backend.Domain.Interfaces.Services;

namespace backend.Application.Services;

public class ContractService : IContractService
{
  private readonly IContractQueryRepository _contractQueryRepository;
  private readonly IContractRepository _contractRepository;
  private readonly IUnitOfWork _unitOfWork;
  public ContractService(
    IContractQueryRepository contractQueryRepository,
    IContractRepository contractRepository,
    IUnitOfWork unitOfWork
    )
  {
    _contractQueryRepository = contractQueryRepository;
    _contractRepository = contractRepository;
    _unitOfWork = unitOfWork;
  }

  public async Task<PagedResponse<PublicContractDto>> GetPublicContractsAsync(QueryParams requestParams)
  {
    ValidateQueryParams(requestParams);

    return await _contractQueryRepository.GetPublicContractsAsync(requestParams);
  }
  public async Task<PagedResponse<PentesterContractDto>> GetPentesterContractsAsync(Guid userId, QueryParams requestParams)
  {
    ValidateQueryParams(requestParams, userId);

    return await _contractQueryRepository.GetPentesterContractsAsync(userId, requestParams);
  }
  public async Task<PagedResponse<CompanyContractDto>> GetCompanyContractsAsync(Guid userId, QueryParams requestParams)
  {
    ValidateQueryParams(requestParams, userId);

    return await _contractQueryRepository.GetCompanyContractsAsync(userId, requestParams);
  }
  public async Task<ContractDetailsDto> GetContractDetailsAsync(long contractId, Guid? userId)
  {
    if(contractId <= 0)
      throw new ValueOutOfRangeAppException();

    if(userId is not null && userId == Guid.Empty)
      throw new UnauthorizedAppException();

    return await _contractQueryRepository.GetContractDetailsAsync(contractId, userId)
      ?? throw new NotFoundAppException();
  }

  public async Task CreateContractAsync(Guid authorId, ContractRequestDto request)
  {
    var contract = new Contract(authorId, request.Title, request.Description, request.Price, request.Deadline);

    await _contractRepository.AddContractAsync(contract);
    await _unitOfWork.SaveChangesAsync();

    return;
  }

  public async Task CloseContractAsync(Guid userId, long contractId)
  {
    if (userId == Guid.Empty)
      throw new BadRequestAppException();
    if (contractId <= 0)
      throw new ValueOutOfRangeAppException();

    var contract = await _contractRepository.GetContractByIdAsync(contractId)
      ?? throw new NotFoundAppException();

    if (contract.AuthorId != userId)
      throw new UnauthorizedAppException();

    contract.CancelContract();
    await _unitOfWork.SaveChangesAsync();
  }

  public async Task UpdateContractAsync(Guid userId, long contractId, UpdateContractDto request)
  {
    if (userId == Guid.Empty)
      throw new BadRequestAppException();
    if (contractId <= 0)
      throw new ValueOutOfRangeAppException();

    var contract = await _contractRepository.GetContractByIdAsync(contractId)
      ?? throw new NotFoundAppException();

    if (contract.AuthorId != userId)
      throw new UnauthorizedAppException();

    if (!string.IsNullOrWhiteSpace(request.Title) || !string.IsNullOrWhiteSpace(request.Description))
      contract.UpdateContractDetails(request.Title, request.Description);

    if (request.Price.HasValue)
      contract.UpdatePrice(request.Price.Value);

    if (request.NewDeadline.HasValue)
      contract.ExtendDeadline(request.NewDeadline.Value);

    await _unitOfWork.SaveChangesAsync();
  }

  public async Task<List<ContractApplicationsDto>> GetContractApplicationsAsync(Guid userId, long contractId)
  {
    if (userId == Guid.Empty)
      throw new BadRequestAppException();
    if (contractId <= 0)
      throw new ValueOutOfRangeAppException();

    var contract = await _contractRepository.GetContractByIdAsync(contractId)
      ?? throw new NotFoundAppException();

    if (contract.AuthorId != userId)
      throw new UnauthorizedAppException();

    var applications = await _contractQueryRepository.GetContractApplicationsAsync(contractId);

    return applications;
  }

  public async Task ApplyToContractAsync(Guid candidateId, long contractId)
  {
    if (candidateId == Guid.Empty || contractId <= 0)
      throw new BadRequestAppException("Invalid candidate ID or contract ID.");

    var hasApplied = await _contractRepository.HasAlreadyAppliedAsync(contractId, candidateId);
    if (hasApplied)
      throw new InvalidOperationAppException("You have already applied to this contract.");

    var contract = await _contractRepository.GetContractByIdAsync(contractId)
      ?? throw new NotFoundAppException();

    if (contract.AuthorId == candidateId)
      throw new InvalidOperationAppException("You cannot apply to your own contract.");

    if (contract.IsExpired())
      throw new InvalidOperationAppException("Cannot apply to an expired contract.");

    if (contract.ContractStatus != ContractStatus.Open)
      throw new InvalidOperationAppException("Cannot apply to a non-open contract.");

    var application = new ContractApplication(contractId, candidateId);
    await _contractRepository.AddApplicationAsync(application);
    await _unitOfWork.SaveChangesAsync();
  }

  private void ValidateQueryParams(QueryParams queryParams, Guid? userId = null)
  {
    if(userId != null && userId == Guid.Empty)
      throw new UnauthorizedAppException();
    
    if (queryParams.Page <= 0 || queryParams.PageSize <= 0 || queryParams.PageSize > 50)
      throw new ValueOutOfRangeAppException("Invalid request params");

    if (!string.IsNullOrWhiteSpace(queryParams.Search) && queryParams.Search.Length > 100)
      throw new ValueOutOfRangeAppException("Invalid request params 'search'");
  }

}