using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Commands;
using backend.Application.Validators;
using backend.Domain.Interfaces;

namespace backend.Domain.Policies;

public class RegisterPolicy : IRegisterPolicy
{


  public Result CanRegisterPentester(bool emailAlreadyExists, bool nicknameAlreadyExists, RegisterPentesterCommand req)
  {
    if (emailAlreadyExists)
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.BadRequest,
        "Błędne dane",
        HttpResponseState.BadRequest
      ));

    if (nicknameAlreadyExists)
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.BadRequest,
        "Taki pseudonim już istnieje",
        HttpResponseState.BadRequest
      ));

    if (!CredentialsFormatValidator.IsValidEmailFormat(req.Email))
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.BadRequest,
        "Zły format: Email",
        HttpResponseState.BadRequest
      ));

    if (!CredentialsFormatValidator.IsValidPassword(req.Password))
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.BadRequest,
        "Zły format: Hasło",
        HttpResponseState.BadRequest
      ));

    if (CredentialsFormatValidator.ContainsAnyOf(req.Password, [req.Email, req.FirstName, req.LastName]))
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.BadRequest,
        "Zły format: Hasło zawiera Email i/lub Imię i/lub Nazwisko",
        HttpResponseState.BadRequest
      ));

    return Result.Success();
  }
  public Result CanRegisterCompany(bool emailAlreadyExists, RegisterCompanyCommand req)
  {
    if (emailAlreadyExists)
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.BadRequest,
        "Błędne dane",
        HttpResponseState.BadRequest
      ));

    if (!CredentialsFormatValidator.IsValidEmailFormat(req.Email))
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.BadRequest,
        "Zły format: Email",
        HttpResponseState.BadRequest
      ));

    if (!CredentialsFormatValidator.IsValidPassword(req.Password))
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.BadRequest,
        "Zły format: Hasło",
        HttpResponseState.BadRequest
      ));

    if (CredentialsFormatValidator.ContainsAnyOf(req.Password, [req.Email, req.Name]))
      return Result.Failure(new Error(
        DomainErrorCodes.AuthCodes.BadRequest,
        "Zły format: Hasło zawiera Email i/lub Nazwę Firmy",
        HttpResponseState.BadRequest
      ));

    return Result.Success();
  }
}
