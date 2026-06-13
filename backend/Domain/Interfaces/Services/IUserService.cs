public interface IUserService
{
  public Task<PagedResponse<UserResponsePublicDto>> GetAllAsync(int page, int pageSize, string? search = null);
  public Task<UserResponsePublicDto> GetUserByIdAsync(Guid userId, Guid currentUser);
  public Task<UserResponsePrivateDto> GetCurrentUserAsync(Guid userId);
  public Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status = null);
  public Task<List<UserApplicationsDto>> GetCurrentUserApplicationsAsync(Guid userId, ContractApplicationStatus? status);
  public Task<UserSummaryDto> GetCurrentUserSummaryAsync(Guid userId);

  public Task UpdateUserAsync(Guid userId, UpdateUserDto request);
  public Task DeleteUserAsync(Guid userId);
}