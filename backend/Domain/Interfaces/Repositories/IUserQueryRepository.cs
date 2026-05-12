public interface IUserQueryRepository
{
  Task<PagedResponse<UserResponseDto>> GetAllAsync(int page, int pageSize, string? search = null);
  Task<UserResponseDto> GetUserByIdAsync(Guid userId);
  Task<UserResponseDto> GetCurrentUserByIdAsync(Guid userId);
}