public class UserCommandService
{
  private readonly IUserRepository _userCommandRepository;
  private readonly IPasswordHasher _hasher;
  private readonly IUnitOfWork _unitOfWork;

  public UserCommandService(
    IUserRepository userCommandRepository,
    IPasswordHasher hasher,
    IUnitOfWork unitOfWork
    )
  {
    _userCommandRepository = userCommandRepository;
    _hasher = hasher;
    _unitOfWork = unitOfWork;
  }

  public async Task UpdateUserAsync(Guid userId, UpdateUserDto request)
  {
    var user = await _userCommandRepository.GetByIdAsync(userId)
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
    var user = await _userCommandRepository.GetByIdAsync(userId)
      ?? throw new NotFoundAppException();

    user.DeleteAccount();
    await _unitOfWork.SaveChangesAsync();
  }

}
