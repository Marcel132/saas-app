using backend.Application.Features.Auth.Commands;
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
      policy.CanRegisterPentester(true, false, CreatePentesterCommand())
    );
  }
  [Test]
  public void Validate_NicknameAlreadyExists_Pentester_ThrowsBadRequest()
  {
    var policy = new RegisterPolicy();

    Assert.Throws<BadRequestAppException>(() =>
      policy.CanRegisterPentester(false, true, CreatePentesterCommand())
    );
  }
  [Test]
  public void Validate_EmailAlreadyExists_Company_ThrowsBadRequest()
  {
    var policy = new RegisterPolicy();

    Assert.Throws<BadRequestAppException>(() =>
      policy.CanRegisterCompany(true, CreateCompanyCommand())
    );
  }

  [Test]
  public void Validate_EmailIsInvalidFormat_Any_ThrowsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var command = CreatePentesterCommand(email: "testagmail.com");

    Assert.Throws<InvalidFormatAppException>(() =>
      policy.CanRegisterPentester(false, false, command)
    );
  }

  [Test]
  public void Validate_PasswordIsInvalidFormat_ThrowsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var command = CreatePentesterCommand(email: "testagmail.com", password: "hereisnoupperletter_123");

    Assert.Throws<InvalidFormatAppException>(() =>
      policy.CanRegisterPentester(false, false, command)
    );
  }

  [Test]
  public void Validate_PasswordContainsEmail_ThrowsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var command = CreatePentesterCommand(email: "test@gmail.com", password: "Test@gmail.com123");

    Assert.Throws<InvalidFormatAppException>(() =>
      policy.CanRegisterPentester(false, false, command)
    );
  }

  [Test]
  public void Validate_PasswordContainsName_ThrowsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var command = CreateCompanyCommand(
      email: "test@gmail.com",
      password: "JanPolSa123__",
      name: "JanPolSa"
    );

    Assert.Throws<InvalidFormatAppException>(() =>
      policy.CanRegisterCompany(false, command)
    );
  }


  private static RegisterPentesterCommand CreatePentesterCommand(
    string email = "test@gmail.com",
    string password = "Password123!",
    string firstName = "Jan",
    string lastName = "Kowalski",
    string nickname = "janek"
  )
  {
    return new RegisterPentesterCommand(
        Email: email,
        Password: password,
        FirstName: firstName,
        LastName: lastName,
        NickName: nickname,
        Phone: "123456789",
        City: "Warsaw",
        Country: "Poland",
        PostalCode: "00-001",
        Street: "Main Street 1",
        Bio: null,
        GithubUrl: null,
        LinkedinUrl: null,
        ExperienceLevel: ExperienceLevel.None,
        IpAddress: "127.0.0.1",
        UserAgent: "NUnit"
    );
  }
  private static RegisterCompanyCommand CreateCompanyCommand(
    string email = "test@gmail.com",
    string password = "Password123!",
    string nip = "1234567890",
    string name = "DarPol S.A"
  )
  {
    return new RegisterCompanyCommand(
        Email: email,
        Password: password,
        Nip: nip,
        Name: name,
        Phone: "123456789",
        City: "Warsaw",
        Country: "Poland",
        PostalCode: "00-001",
        Street: "Main Street 1",
        Bio: null,
        WebsiteUrl: null,
        IpAddress: "127.0.0.1",
        UserAgent: "NUnit"
    );
  }
}