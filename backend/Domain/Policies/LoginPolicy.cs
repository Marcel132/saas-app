using backend.Domain.Entities;
using backend.Domain.Interfaces;

namespace backend.Domain.Policies;

public class LoginPolicy : ILoginPolicy
{
  public void EnsureCanLogin(User? user)
  {
    // * Note: To avoid user enumaration, every exception shoud be the same for non existing user and blocked/inactive accounts.
    if (user == null)
      throw new InvalidCredentialsAppException("Nieprawidłowy email lub hasło");

    if (!user.IsActive)
      throw new InvalidCredentialsAppException("Nieprawidłowy email lub hasło");

    if (!user.CanLogin())
      throw new InvalidCredentialsAppException("Nieprawidłowy email lub hasło");
  }
}