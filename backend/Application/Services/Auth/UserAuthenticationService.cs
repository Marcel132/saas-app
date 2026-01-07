public class UserAuthenticationService
{
  private readonly IUserRepository _users;
  private readonly IPasswordHasher _hasher;
  private readonly ILoginPolicy _policy;

  private const int MaxAttempts = 5;
  private static readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(15);

  public UserAuthenticationService(
    IUserRepository users,
    IPasswordHasher hasher,
    ILoginPolicy policy
  )
  {
    _users = users;
    _hasher = hasher;
    _policy = policy;
  }

  public async Task<User> AuthenticateAsync(string email, string password)
  {
    var user = await _users.GetByEmailAsync(email)
      ?? throw new InvalidCredentialsAppException();

    _policy.EnsureCanLogin(user);

    if (!_hasher.Verify(password, user.PasswordHash))
    {
      user.RegisterFailedLoginAttempt(MaxAttempts, BlockDuration);
      await _users.UpdateAsync(user);
      throw new InvalidCredentialsAppException();
    }

    user.ResetFailedLoginAttempts();
    await _users.UpdateAsync(user);

    return user;
  }
}