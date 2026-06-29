using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace backend.Application.Validators;

public static class CredentialsFormatValidator
{
  private static readonly EmailAddressAttribute EmailValidator = new();
  private static readonly Regex PasswordRegex = new(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,64}$",
    RegexOptions.Compiled
  );

  public static bool IsValidEmailFormat(string email)
    => EmailValidator.IsValid(email);

  public static bool IsValidPassword(string password)
    => PasswordRegex.IsMatch(password);

  public static bool ContainsAnyOf(string target, params string[] sensitiveValues)
  {
    foreach (var value in sensitiveValues)
    {
      if(string.IsNullOrWhiteSpace(value))
        continue;

      if(target.Contains(value, StringComparison.OrdinalIgnoreCase))
        return true;
    }

    return false;
  }
}