using backend.Api.Controllers.Auth.DTOs;
using backend.Application.Validators;
using backend.Domain.Interfaces;

namespace backend.Domain.Policies;

public class RegisterPolicy : IRegisterPolicy
{


  public void EnsureCanRegisterPentester(bool emailAlreadyExists, bool nicknameAlreadyExists, RegisterPentesterRequestDto req)
  {
    if (emailAlreadyExists)
      throw new BadRequestAppException("Niepoprawne dane");

    if (nicknameAlreadyExists)
      throw new BadRequestAppException("Taki pseudonim już istnieje");

    if (!CredentialsFormatValidator.IsValidEmailFormat(req.Email))
      throw new InvalidFormatAppException("Zły format: Email");

    if (!CredentialsFormatValidator.IsValidPassword(req.Password))
      throw new InvalidFormatAppException("Zły format: Hasło");

    if (CredentialsFormatValidator.ContainsAnyOf(req.Password, [req.Email, req.FirstName, req.LastName]))
      throw new InvalidFormatAppException("Zły format: Hasło zawiera: {Email i/lub Imię i/lub Hasło}");

  }
  public void EnsureCanRegisterCompany(bool emailAlreadyExists, RegisterCompanyRequestDto req)
  {
    if (emailAlreadyExists)
      throw new BadRequestAppException("Niepoprawne dane");

    if (!CredentialsFormatValidator.IsValidEmailFormat(req.Email))
      throw new InvalidFormatAppException("Zły format: Email");

    if (!CredentialsFormatValidator.IsValidPassword(req.Password))
      throw new InvalidFormatAppException("Zły format: Hasło");

    if (CredentialsFormatValidator.ContainsAnyOf(req.Password, [req.Email, req.Name]))
      throw new InvalidFormatAppException("Zły format: Hasło zawiera: {Email i/lub Nazwę}");
  }
}
