using backend.Api.Controllers;
using backend.Api.Controllers.Contracts.DTOs;

namespace backend.Domain.Interfaces.Repositories;

public interface IContractQueryRepository
{
  public Task<PagedResponse<PublicContractDto>> GetPublicContractsAsync(QueryParams queryParams);
  public Task<PagedResponse<OpenContractDto>> GetOpenContractsAsync(Guid userId, QueryParams queryParams);
  public Task<PagedResponse<CompanyContractDto>> GetCompanyContractsAsync(Guid userId, QueryParams queryParams);
  public Task<ContractDetailsDto?> GetContractDetailsAsync(long contractId, Guid? userId);
  public Task<List<ContractApplicationsDto>> GetContractApplicationsAsync(long contractId);

}