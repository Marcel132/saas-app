using backend.Api.Controllers;
using backend.Api.Controllers.Contracts.DTOs;

namespace backend.Domain.Interfaces.Features;

public interface IContractService
{
  public Task<PagedResponse<PublicContractDto>> GetPublicContractsAsync(QueryParams requestParams, CancellationToken ct = default);
  public Task<PagedResponse<OpenContractDto>> GetOpenContractsAsync(Guid userId, QueryParams requestParams, CancellationToken ct = default);
  public Task<PagedResponse<CompanyContractDto>> GetCompanyContractsAsync(Guid userId, QueryParams requestParams, CancellationToken ct = default);
  public Task<ContractDetailsDto> GetContractDetailsAsync(long contractId, Guid? userId, CancellationToken ct = default);
  public Task<List<ContractApplicationsDto>> GetContractApplicationsAsync(Guid userId, long contractId, CancellationToken ct = default);

  public Task CreateContractAsync(Guid authorId, ContractRequestDto request);
  public Task CloseContractAsync(Guid userId, long contractId);
  public Task UpdateContractAsync(Guid userId, long contractId, UpdateContractDto request);
  public Task ApplyToContractAsync(Guid candidateId, long contractId);
}