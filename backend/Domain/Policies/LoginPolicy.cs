namespace backend.Domain.Policies;

using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Entities;
using backend.Domain.Interfaces;

public class LoginPolicy : ILoginPolicy
{
  public Result CanLogin(User? user)
  {
    // * Note: żeby uniknąć user enumeration, błąd musi być identyczny dla braku usera i zablokowanego/nieaktywnego konta.
    if (user is null)
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.InvalidCredentials, "Nieprawidłowy email lub hasło", HttpResponseState.Unauthorized));

    if (!user.IsActive)
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.InvalidCredentials, "Nieprawidłowy email lub hasło", HttpResponseState.Unauthorized));

    if (!user.CanLogin())
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.InvalidCredentials, "Nieprawidłowy email lub hasło", HttpResponseState.Unauthorized));

    return Result.Success();
  }
}