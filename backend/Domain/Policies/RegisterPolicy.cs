public class RegisterPolicy : IRegisterPolicy
{

  public void EnsureCanRegister(bool emailAlreadyExists, RegisterRequestDto request)
  {
    if(emailAlreadyExists)
      throw new BadRequestAppException();

    if(IsPasswordWeak(request))
      throw new InvalidFormatAppException();

    if(HasInvalidCompanyData(request))
      throw new InvalidFormatAppException();
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

}