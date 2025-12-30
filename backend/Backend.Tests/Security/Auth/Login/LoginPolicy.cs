public class LoginPolicy
{
  public void Validate(UserLoginDataDto? user, string password)
  {
    if(user == null)
      throw new UnauthorizedAccessException("Invalid credentials");
    
    if(!BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
      throw new UnauthorizedAccessException("Invalid credentials");

    if(!user.IsActive)
      throw new ForbiddenAppException("Account blocked");

  }
}