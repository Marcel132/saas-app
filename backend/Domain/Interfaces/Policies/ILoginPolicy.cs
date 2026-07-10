namespace backend.Domain.Interfaces;

using backend.Application.Abstractions.CQRS;
using backend.Domain.Entities;

public interface ILoginPolicy
{
  Result CanLogin(User? user);
}