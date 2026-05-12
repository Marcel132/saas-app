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

  public async Task<PagedResponse<UserResponseDto>> GetAllAsync(int page, int pageSize, string? search = null)
  {
    return await _userQueryRepository.GetAllAsync(page, pageSize, search);
  }

  public async Task<UserResponseDto> GetUserByIdAsync(Guid userId, Guid currentUserId, bool canReadAll)  
  {
    var user = await _userQueryRepository.GetUserByIdAsync(userId);

    if (!canReadAll && userId != currentUserId)
      throw new NotFoundAppException();

    return user;
  }

  public async Task<UserResponseDto> GetCurrentUserByIdAsync(Guid userId)
  {
    return await _userQueryRepository.GetCurrentUserByIdAsync(userId);
  }
}