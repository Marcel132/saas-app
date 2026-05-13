using Microsoft.EntityFrameworkCore;

public class UserQueryService
{ 
  private readonly IUserQueryRepository _userQueryRepository;

  public UserQueryService
  (
    IUserQueryRepository userQueryRepository
  )
  {
    _userQueryRepository = userQueryRepository;
  }

  public async Task<PagedResponse<UserResponsePublicDto>> GetAllAsync(int page, int pageSize, string? search = null)
  {
    return await _userQueryRepository.GetAllAsync(page, pageSize, search);
  }

  public async Task<UserResponsePublicDto> GetUserByIdAsync(Guid userId, Guid currentUserId)  
  {
    var user = await _userQueryRepository.GetUserByIdAsync(userId);

    // TODO: Create log with userId and currentUserId (who requested the data) for auditing purposes

    return user;
  }

  public async Task<UserResponsePrivateDto> GetCurrentUserByIdAsync(Guid userId)
  {
    return await _userQueryRepository.GetCurrentUserByIdAsync(userId);
  }
}