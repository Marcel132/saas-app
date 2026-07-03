using backend.Api.Controllers;
using backend.Api.Controllers.Users.DTOs;
using backend.Domain.Entities.Enum;

namespace backend.Domain.Interfaces.Repositories;

public interface IUserQueryRepository
{
  // GetAllAsync (admin) 

  Task<RoleType> GetRoleTypeAsync(Guid userId);

  Task<UserPublicPentesterDto> GetPentesterByIdAsync(Guid userId, CancellationToken ct);

  Task<PentesterPrivateDto> GetCurrentPentesterAsync(Guid userId);
  Task<CompanyPrivateDto> GetCurrentCompanyAsync(Guid userId);

  Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status = null, CancellationToken ct = default);
  Task<List<UserApplicationsDto>> GetApplicationsAsync(Guid userId, ContractApplicationStatus? status, CancellationToken ct = default);
  Task<UserSummaryDto> GetSummary(Guid userId, CancellationToken ct = default);
}