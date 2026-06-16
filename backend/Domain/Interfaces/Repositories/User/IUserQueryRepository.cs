using backend.Api.Controllers;
using backend.Api.Controllers.Users.DTOs;
using backend.Domain.Entities.Enum;

namespace backend.Domain.Interfaces.Repositories;

public interface IUserQueryRepository
{
  public Task<PagedResponse<UserResponsePublicDto>> GetAllAsync(int page, int pageSize, string? search = null);
  public Task<UserResponsePublicDto> GetUserByIdAsync(Guid userId);
  public Task<UserResponsePrivateDto> GetCurrentUserByIdAsync(Guid userId);
  public Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status = null);
  public Task<List<UserApplicationsDto>> GetApplicationsAsync(Guid userId, ContractApplicationStatus? status);
  public Task<UserSummaryDto> GetSummary(Guid userid);

}