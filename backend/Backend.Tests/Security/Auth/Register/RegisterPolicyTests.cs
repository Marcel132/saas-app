using backend.Api.Controllers.Auth.DTOs;
using backend.Domain.Entities.Enum;
using backend.Domain.Policies;
using NUnit.Framework;

namespace backend.Backend.Tests.Security.Auth;

public class RegisterPolicyTests
{
  [Test]
  public void Validate_EmailAlreadyExists_Pentester_ThrowsBadRequest()
  {
    var policy = new RegisterPolicy();

    Assert.Throws<BadRequestAppException>(() =>
      policy.EnsureCanRegisterPentester(true, false, new RegisterPentesterRequestDto())
    );
  }
  [Test]
  public void Validate_NicknameAlreadyExists_Pentester_ThrowsBadRequest()
  {
    var policy = new RegisterPolicy();

    Assert.Throws<BadRequestAppException>(() =>
      policy.EnsureCanRegisterPentester(false, true, new RegisterPentesterRequestDto())
    );
  }
  [Test]
  public void Validate_EmailAlreadyExists_Company_ThrowsBadRequest()
  {
    var policy = new RegisterPolicy();

    Assert.Throws<BadRequestAppException>(() =>
      policy.EnsureCanRegisterCompany(true, new RegisterCompanyRequestDto())
    );
  }

  [Test]
  public void Validate_EmailIsInvalidFormat_Any_ThrowsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var user = new RegisterPentesterRequestDto
    {
      Email = "testagmail.com"
    };

    Assert.Throws<InvalidFormatAppException>(() =>
      policy.EnsureCanRegisterPentester(false, false, user)
    );
  }

  [Test]
  public void Validate_PasswordIsInvalidFormat_ThrowsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var user = new RegisterPentesterRequestDto
    {
      Email = "test@gmail.com",
      Password = "thereisnoupperletter_123"
    };

    Assert.Throws<InvalidFormatAppException>(() =>
      policy.EnsureCanRegisterPentester(false, false, user)
    );
  } 

  [Test]
  public void Validate_PasswordContainsEmail_ThrowsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var user = new RegisterPentesterRequestDto
    {
      Email = "test@gmail.com",
      Password = "Test@gmail.com123"
    };

    Assert.Throws<InvalidFormatAppException>(() =>
      policy.EnsureCanRegisterPentester(false, false, user)
    );
  } 

  [Test]
  public void Validate_PasswordContainsName_ThrowsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var user = new RegisterCompanyRequestDto
    {
      Email = "test@gmail.com",
      Password = "JanPolSa123__",
      Name = "JanPolSa"
    };

    Assert.Throws<InvalidFormatAppException>(() =>
      policy.EnsureCanRegisterCompany(false, user)
    );
  } 
}