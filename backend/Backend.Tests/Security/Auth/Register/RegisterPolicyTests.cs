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
      FirstName = "John",
      LastName = "Doe",
      SpecializationType = [Specialization.Audit, Specialization.Pentester],
      CompanyName = "PorkAndCheese"
    };

    Assert.Throws<InvalidFormatAppException>(() =>
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
      SpecializationType = [Specialization.Audit, Specialization.Pentester],
      CompanyName = "PorkAndCheese"
    };

    Assert.Throws<InvalidFormatAppException>(() =>
      policy.EnsureCanRegister(false, req)
    );
  }
}