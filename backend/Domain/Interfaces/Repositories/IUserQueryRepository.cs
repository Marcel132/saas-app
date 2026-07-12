using backend.Api.Controllers.Users.DTOs;
using backend.Domain.Entities.Enum;

namespace backend.Domain.Interfaces.Repositories;

public interface IUserQueryRepository
{
  // GetAllAsync (admin) 

  Task<RoleType> GetRoleTypeAsync(Guid userId, CancellationToken ct);

  Task<UserPublicPentesterDto> GetPentesterByIdAsync(Guid userId, CancellationToken ct);

  Task<PentesterPrivateDto> GetCurrentPentesterAsync(Guid userId, CancellationToken ct);
  Task<CompanyPrivateDto> GetCurrentCompanyAsync(Guid userId, CancellationToken ct);

  Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status, CancellationToken ct);
  Task<List<UserApplicationsDto>> GetApplicationsAsync(Guid userId, ContractApplicationStatus? status, CancellationToken ct);
  Task<UserSummaryDto> GetSummary(Guid userId, CancellationToken ct);
}