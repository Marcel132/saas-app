using backend.Api.Controllers;
using backend.Api.Controllers.Users.DTOs;
using backend.Domain.Entities.Enum;

namespace backend.Domain.Interfaces.Repositories;

public interface IUserQueryRepository
{
  // GetAllAsync (admin) 

  Task<RoleType> GetRoleTypeAsync(Guid userId);

  Task<UserPublicPentesterDto> GetPentesterByIdAsync(Guid userId);

  Task<PentesterPrivateDto> GetCurrentPentesterAsync(Guid userId);
  Task<CompanyPrivateDto> GetCurrentCompanyAsync(Guid userId);

  Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status = null);
  Task<List<UserApplicationsDto>> GetApplicationsAsync(Guid userId, ContractApplicationStatus? status);
  Task<UserSummaryDto> GetSummary(Guid userId);
}