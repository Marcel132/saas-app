using NUnit.Framework;

public class RegisterPolicyTests
{
  [Test]
  public void Validate_EmailAlreadyExists_ThrowsBadRequest()
  {
    var policy = new RegisterPolicy();

    Assert.Throws<BadRequestAppException>(() => 
      policy.EnsureCanRegister(true, new RegisterRequestDto())
    );
  }

  [Test]
  public void Validate_IsPasswordWeak_ThrowsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var req = new RegisterRequestDto
    {
      Email = "test@example.com",
      Password = "JohnTestPassword!123",
      Nickname = "JohnDoe",
      FirstName = "John",
      LastName = "Doe",
      SpecializationType = [Specialization.ApiSecurity, Specialization.CloudSecurity],
      CompanyName = null,
      CompanyNip = null
    };

    Assert.Throws<InvalidFormatAppException>(() =>
      policy.EnsureCanRegister(false, req)
    );
  }

  [Test]
  public void Validate_IsSpecializationInvalid_ThrowsBadRequest()
  {
    var policy = new RegisterPolicy();

    var req = new RegisterRequestDto
    {
      Email = "test@example.com",
      Password = "StrongPassword!123",
      Nickname = "JohnDoe",
      FirstName = "John",
      LastName = "Doe",
      SpecializationType = [],
      CompanyName = null,
      CompanyNip = null
    };

    Assert.Throws<BadRequestAppException>(() =>
      policy.EnsureCanRegister(false, req)
    );
  }

  [Test]
  public void Validate_InvalidCompanyData_ThrowsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var req = new RegisterRequestDto
    {
      Email = "test@example.com",
      Password = "StrongPassword!123",
      FirstName = "John",
      LastName = "Doe",
      Nickname = "JohnDoe",
      SpecializationType = [Specialization.RedTeam],
      CompanyName = "PorkAndCheese",
      CompanyNip = null
    };

    Assert.Throws<InvalidFormatAppException>(() =>
      policy.EnsureCanRegister(false, req)
    );
  }
  public void RequestIsValid_Success()
  {
    var policy = new RegisterPolicy();

    var req = new RegisterRequestDto
    {
      Email = "test@example.com",
      Password = "StrongPassword!123",
      FirstName = "John",
      LastName = "Doe",
      Nickname = "JohnDoe",
      SpecializationType = [Specialization.WebSecurity],
      CompanyName = "PorkAndCheese",
      CompanyNip = "123-456-789"
    };

    Assert.DoesNotThrow(() =>
      policy.EnsureCanRegister(false, req)
    );
  }

}