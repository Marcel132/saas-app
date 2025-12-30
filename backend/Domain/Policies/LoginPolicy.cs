public class LoginPolicy : ILoginPolicy
{
  public void EnsureCanLogin(UserLoginDataDto? user, string password)
  {
    if(user == null)
      throw new InvalidCredentialsAppException();
    
    if(!BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
      throw new InvalidCredentialsAppException();

    if(!user.IsActive)
      throw new ForbiddenAppException();
  }
}