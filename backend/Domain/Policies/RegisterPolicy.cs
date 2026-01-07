public class RegisterPolicy : IRegisterPolicy
{

  public void EnsureCanRegister(bool emailAlreadyExists, RegisterRequestDto request)
  {
    if(emailAlreadyExists)
      throw new BadRequestAppException();

    if(IsPasswordWeak(request))
      throw new InvalidFormatAppException();

    if(IsSpecializationInvalid(request))
      throw new BadRequestAppException(); 

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