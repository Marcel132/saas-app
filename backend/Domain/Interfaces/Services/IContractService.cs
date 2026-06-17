using backend.Api.Controllers;
using backend.Api.Controllers.Contracts.DTOs;

namespace backend.Domain.Interfaces.Services;

public interface IContractService
{
  public Task<PagedResponse<PublicContractDto>> GetPublicContractsAsync(QueryParams requestParams);
  public Task<PagedResponse<PentesterContractDto>> GetPentesterContractsAsync(Guid userId, QueryParams requestParams);
  public Task<PagedResponse<CompanyContractDto>> GetCompanyContractsAsync(Guid userId, QueryParams requestParams);
  public Task<ContractDetailsDto> GetContractDetailsAsync(long contractId, Guid? userId);

  public Task CreateContractAsync(Guid authorId, ContractRequestDto request);
  public Task CloseContractAsync(Guid userId, long contractId);
  public Task UpdateContractAsync(Guid userId, long contractId, UpdateContractDto request);
  public Task<List<ContractApplicationsDto>> GetContractApplicationsAsync(Guid userId, long contractId);
  public Task ApplyToContractAsync(Guid candidateId, long contractId);
}