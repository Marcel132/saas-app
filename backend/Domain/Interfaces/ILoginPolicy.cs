public interface ILoginPolicy
{
  void EnsureCanLogin(User? user);
}