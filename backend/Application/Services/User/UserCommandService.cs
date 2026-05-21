public class UserCommandService
{
  private readonly IUserRepository _userCommandRepository;
  private readonly IPasswordHasher _hasher;

  public UserCommandService(
    IUserRepository userCommandRepository,
    IPasswordHasher hasher)
  {
    _userCommandRepository = userCommandRepository;
    _hasher = hasher;
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

    user.UpdateUserData(
      request.FirstName,
      request.LastName,
      request.Nickname,
      request.PhoneNumber,
      request.Skills,
      request.City,
      request.Country,
      request.PostalCode,
      request.Street,
      request.CompanyName,
      request.CompanyNip
    );

    await _userCommandRepository.SaveChangesAsync();
  }

  public async Task DeleteUserAsync(Guid userId)
  {
    var user = await _userCommandRepository.GetByIdAsync(userId)
      ?? throw new NotFoundAppException();

    user.DeleteAccount();
    await _userCommandRepository.SaveChangesAsync();
  }

}
