public class LoginPolicy : ILoginPolicy
{
  public void EnsureCanLogin(User? user)
  {
    // * Note: To avoid user enumaration, every exception shoud be the same for non existing user and blocked/inactive accounts.
    if(user == null)
      throw new InvalidCredentialsAppException();

    if(!user.IsActive)
      throw new InvalidCredentialsAppException();
  
    if(!user.CanLogin())
      throw new InvalidCredentialsAppException();    
  }
}