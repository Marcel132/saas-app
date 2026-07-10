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

  public async Task<UserPublicPentesterDto> GetPentesterByIdAsync(Guid userId, Guid currentUserId, CancellationToken ct)
  {
    if (userId == Guid.Empty)
      throw new BadRequestAppException();

    var pentester = await _userQueryRepo.GetPentesterByIdAsync(userId, ct);

    // TODO: Create log with userId and currentUserId (who requested the data) for auditing purposes

    return pentester;
  }

  public async Task<object> GetCurrentUserAsync(Guid userId)
  {
    var roleType = await _userQueryRepo.GetRoleTypeAsync(userId);

    return roleType switch
    {
      RoleType.Pentester => (object)await _userQueryRepo.GetCurrentPentesterAsync(userId),
      RoleType.Company => (object)await _userQueryRepo.GetCurrentCompanyAsync(userId),
      _ => throw new InvalidOperationAppException()
    };
  }

  public async Task<List<UserContractsDto>> GetCurrentUserContractsAsync(Guid userId, ContractStatus? status, CancellationToken ct)
  {
    return await _userQueryRepo.GetCurrentUserContractsAsync(userId, status, ct);
  }

  public async Task<List<UserApplicationsDto>> GetCurrentUserApplicationsAsync(Guid userId, ContractApplicationStatus? status, CancellationToken ct)
  {
    return await _userQueryRepo.GetApplicationsAsync(userId, status, ct);
  }

  public async Task<UserSummaryDto> GetCurrentUserSummaryAsync(Guid userId, CancellationToken ct)
  {
    return await _userQueryRepo.GetSummary(userId, ct);
  }

  public async Task UpdatePentesterAsync(Guid userId, UpdatePentesterDto request)
  {
    var user = await _userRepo.GetByIdAsync(userId)
      ?? throw new NotFoundAppException();

    if (request.SpecializationType != null)
    {
      user.ClearAllPentesterSpecializations();

      foreach (var spec in request.SpecializationType)
        user.AddPentesterSpecialization(spec);
    }

    user.UpdatePentesterProfile(
      request.FirstName,
      request.LastName,
      request.NickName,
      request.Phone,
      request.Country,
      request.City,
      request.Street,
      request.PostalCode,
      request.Bio,
      request.GithubUrl,
      request.LinkedinUrl
    );

    await _unitOfWork.SaveChangesAsync();
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