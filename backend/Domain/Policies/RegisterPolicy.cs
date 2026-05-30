public class RegisterPolicy : IRegisterPolicy
{

  public void EnsureCanRegister(bool emailAlreadyExists, RegisterRequestDto request)
  {
    if(emailAlreadyExists)
      throw new BadRequestAppException("Podany adres email już istnieje");

    if(string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
      throw new InvalidFormatAppException("Imię i nazwisko są wymagane");
    
    var email = request.Email.Trim().ToLowerInvariant();
    if(!User.IsValidEmailFormat(email))
      throw new InvalidFormatAppException("Niepoprawny format adresu email");

    if(!User.IsValidPasswordFormat(request.Password))
      throw new InvalidFormatAppException("Hasło nie spełnia wymogów bezpieczeństwa");

    if(IsPasswordWeak(request))
      throw new InvalidFormatAppException("Hasło jest za słabe. Nie używaj poufnych danych");

    if(IsSpecializationInvalid(request))
      throw new BadRequestAppException("Wybierz co najmniej jedną specjalizacje"); 

    if(HasInvalidCompanyData(request))
      throw new InvalidFormatAppException("Podaj nazwę oraz NIP firmy");

  }

  private static bool IsPasswordWeak(RegisterRequestDto req)
  {
    return req.Password.Contains(req.Email, StringComparison.OrdinalIgnoreCase)
      || req.Password.Contains(req.FirstName, StringComparison.OrdinalIgnoreCase)
      || req.Password.Contains(req.LastName, StringComparison.OrdinalIgnoreCase);
  }

  private static bool HasInvalidCompanyData(RegisterRequestDto req)
  {
    var hasName = !string.IsNullOrEmpty(req.CompanyName);
    var hasNip  = !string.IsNullOrEmpty(req.CompanyNip);

    return hasName ^ hasNip; 
  }

  private static bool IsSpecializationInvalid(RegisterRequestDto req)
  {
    if(req.SpecializationType == null || !req.SpecializationType.Any())
      return true;
    
    foreach (var spec in req.SpecializationType)
    {
      if (!Enum.IsDefined(typeof(Specialization), spec))
        return true;
    }

    return false;
  }
}
