using backend.Api.Controllers;
using backend.Api.Controllers.Users.DTOs;
using backend.Domain.Entities.Enum;

namespace backend.Domain.Interfaces.Services;

public interface IUserService
{
  // GetAllAsync (admin) 

  public Task<UserPublicPentesterDto> GetPentesterByIdAsync(Guid userId, Guid currentUserId);
  public Task<object> GetCurrentUserAsync(Guid userId);
  public Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status = null);
  public Task<List<UserApplicationsDto>> GetCurrentUserApplicationsAsync(Guid userId, ContractApplicationStatus? status);
  public Task<UserSummaryDto> GetCurrentUserSummaryAsync(Guid userId);

  public Task UpdatePentesterAsync(Guid userId, UpdatePentesterDto request);
  public Task UpdateCompanyAsync(Guid userId, UpdateCompanyDto request);
  public Task DeleteUserAsync(Guid userId);
}