using backend.Domain.Entities;

namespace backend.Domain.Interfaces;

public interface ILoginPolicy
{
  void EnsureCanLogin(User? user);
}