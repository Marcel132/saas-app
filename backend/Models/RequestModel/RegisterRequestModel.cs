public class RegisterRequestModel
{
  public UsersModel? User { get; set; }
  public UserDataModel? UserData { get; set; }
}

public class TokenAuthModel
{
  public int UserId { get; set; }
  public string? Role { get; set; }
}