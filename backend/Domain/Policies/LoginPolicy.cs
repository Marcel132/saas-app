public class LoginPolicy : ILoginPolicy
{
  public void EnsureCanLogin(User? user)
  {
    if(user == null)
      throw new InvalidCredentialsAppException();

    if(user.LoginBlockedUntil > DateTime.UtcNow)
      throw new AccountBlockedAppException();

    if(!user.IsActive)
      throw new ForbiddenAppException();
  }
}