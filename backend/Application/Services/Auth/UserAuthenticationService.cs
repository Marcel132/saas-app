public class UserAuthenticationService
{
  private readonly IUserRepository _users;
  private readonly IPasswordHasher _hasher;
  private readonly ILoginPolicy _policy;
  private readonly IUnitOfWork _unitOfWork;

  private const int MaxAttempts = 5;
  private static readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(10);

  public UserAuthenticationService(
    IUserRepository users,
    IPasswordHasher hasher,
    ILoginPolicy policy,
    IUnitOfWork unitOfWork
  )
  {
    _users = users;
    _hasher = hasher;
    _policy = policy;
    _unitOfWork = unitOfWork;
  }

  public async Task<User> AuthenticateAsync(string email, string password)
  {
    email = email.Trim().ToLowerInvariant();
    var user = await _users.GetByEmailAsync(email)
      ?? throw new InvalidCredentialsAppException("Błędne dane");

    _policy.EnsureCanLogin(user);

    if (!_hasher.Verify(password, user.PasswordHash))
    {
      user.RegisterFailedLoginAttempt(MaxAttempts, BlockDuration);
      await _unitOfWork.SaveChangesAsync();
      throw new InvalidCredentialsAppException("Błędne dane");
    }

    user.ResetFailedLoginAttempts();
    await _unitOfWork.SaveChangesAsync();

    return user;
  }
}