using backend.Api.Controllers;
using backend.Api.Controllers.Users.DTOs;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;
using backend.Domain.Interfaces.Services;

namespace backend.Application.Services;

public class UserService : IUserService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IUserRepository _userRepo;
  private readonly IUserQueryRepository _userQueryRepo;

  public UserService(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IUserQueryRepository userQueryRepository
  )
  {
    _unitOfWork = unitOfWork;
    _userQueryRepo = userQueryRepository;
    _userRepo = userRepository;
  }

  public async Task<PagedResponse<UserResponsePublicDto>> GetAllAsync(int page, int pageSize, string? search = null)
  {
    return await _userQueryRepo.GetAllAsync(page, pageSize, search);
  }

  public async Task<UserResponsePublicDto> GetUserByIdAsync(Guid userId, Guid currentUserId)
  {
    if (userId == Guid.Empty)
    {
      throw new BadRequestAppException();
    }

    var user = await _userQueryRepo.GetUserByIdAsync(userId);

    // TODO: Create log with userId and currentUserId (who requested the data) for auditing purposes

    return user;
  }

  public async Task<UserResponsePrivateDto> GetCurrentUserAsync(Guid userId)
  {
    return await _userQueryRepo.GetCurrentUserByIdAsync(userId);
  }

  public async Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status = null)
  {
    return await _userQueryRepo.GetCurrentUserContractsAsync(userId, status);
  }

  public async Task<List<UserApplicationsDto>> GetCurrentUserApplicationsAsync(Guid userId, ContractApplicationStatus? status)
  {
    return await _userQueryRepo.GetApplicationsAsync(userId, status);
  }

  public async Task<UserSummaryDto> GetCurrentUserSummaryAsync(Guid userId)
  {
    return await _userQueryRepo.GetSummary(userId);
  }


  public async Task UpdateUserAsync(Guid userId, UpdateUserDto request)
  {
    var user = await _userRepo.GetByIdAsync(userId)
      ?? throw new NotFoundAppException();

    if (request.SpecializationType != null)
    {
      user.ClearSpecializations();
      foreach (var spec in request.SpecializationType)
        user.AddSpecialization(spec);
    }

    user.UpdateUserData(request);

    await _unitOfWork.SaveChangesAsync();
  }

  public async Task DeleteUserAsync(Guid userId)
  {
    var user = await _userRepo.GetByIdAsync(userId)
      ?? throw new NotFoundAppException();

    user.DeleteAccount();
    await _unitOfWork.SaveChangesAsync();
  }

}