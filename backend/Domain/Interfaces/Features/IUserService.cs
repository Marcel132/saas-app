using backend.Api.Controllers.Users.DTOs;
using backend.Domain.Entities.Enum;

namespace backend.Domain.Interfaces.Features;

public interface IUserService
{
  // GetAllAsync (admin) 
  public Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status = null, CancellationToken ct = default);
  public Task<List<UserApplicationsDto>> GetCurrentUserApplicationsAsync(Guid userId, ContractApplicationStatus? status, CancellationToken ct = default);
  public Task<UserSummaryDto> GetCurrentUserSummaryAsync(Guid userId, CancellationToken ct = default);

  public Task UpdatePentesterAsync(Guid userId, UpdatePentesterDto request);
  public Task UpdateCompanyAsync(Guid userId, UpdateCompanyDto request);
  public Task DeleteUserAsync(Guid userId);
}