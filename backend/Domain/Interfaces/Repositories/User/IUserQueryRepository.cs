public interface IUserQueryRepository
{
  Task<PagedResponse<UserResponsePublicDto>> GetAllAsync(int page, int pageSize, string? search = null);
  Task<UserResponsePublicDto> GetUserByIdAsync(Guid userId);
  Task<UserResponsePrivateDto> GetCurrentUserByIdAsync(Guid userId);

  Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status = null);

}