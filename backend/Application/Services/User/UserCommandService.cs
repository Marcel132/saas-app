public class UserCommandService
{
  private readonly IUserRepository _users;
  private readonly IPasswordHasher _hasher;

  public UserCommandService(
    IUserRepository users,
    IPasswordHasher hasher)
  {
    _users = users;
    _hasher = hasher;
  }

  public async Task UpdateUserAsync(Guid userId, UpdateUserDto request)
  {
    var user = await _users.GetByIdAsync(userId)
      ?? throw new NotFoundAppException();

    // if (!string.IsNullOrWhiteSpace(request.Email))
    //   user.ChangeEmail(request.Email);

    // TODO: Check current password; if true, save new password; else 400;
    if (!string.IsNullOrWhiteSpace(request.NewPassword))
      user.ChangePassword(_hasher.Hash(request.NewPassword));

    if (request.SpecializationType != null)
    {
      user.ClearSpecializations();
      foreach (var spec in request.SpecializationType)
        user.AddSpecialization(spec);
    }

    user.UpdateUserData(
      request.FirstName,
      request.LastName,
      request.PhoneNumber,
      request.Skills,
      request.City,
      request.Country,
      request.PostalCode,
      request.Street,
      request.CompanyName,
      request.CompanyNip
    );

    await _users.UpdateAsync(user);
  }

  public async Task DeleteUserAsync(Guid userId)
  {
    var user = await _users.GetByIdAsync(userId)
      ?? throw new NotFoundAppException();

    user.DeleteAccount();

    await _users.UpdateAsync(user);
  }
}