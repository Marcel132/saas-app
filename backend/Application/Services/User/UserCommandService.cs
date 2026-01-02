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
      ?? throw new KeyNotFoundException();

    if (!string.IsNullOrWhiteSpace(request.Email))
      user.ChangeEmail(request.Email);

    if (!string.IsNullOrWhiteSpace(request.Password))
      user.ChangePassword(_hasher.Hash(request.Password));

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
      ?? throw new KeyNotFoundException();

    user.Deactivate();

    await _users.DeleteAsync(user);
  }
}