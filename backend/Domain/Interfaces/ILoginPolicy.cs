public interface ILoginPolicy
{
  void EnsureCanLogin(UserLoginDataDto? user, string password);
}