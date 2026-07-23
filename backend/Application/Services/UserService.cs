using backend.Api.Controllers.Users.DTOs;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;
using backend.Domain.Interfaces.Features;

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

  public async Task UpdateCompanyAsync(Guid userId, UpdateCompanyDto request)
  {
    var user = await _userRepo.GetByIdAsync(userId)
      ?? throw new NotFoundAppException();

    user.UpdateCompanyProfile(
      request.Name,
      request.Phone,
      request.Country,
      request.City,
      request.Street,
      request.PostalCode,
      request.Bio,
      request.WebsiteUrl
    );

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