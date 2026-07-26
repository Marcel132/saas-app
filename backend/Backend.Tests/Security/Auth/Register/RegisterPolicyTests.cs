using backend.Api.Http;
using backend.Application.Features.Auth.Commands;
using backend.Domain.Entities.Enum;
using backend.Domain.Policies;
using NUnit.Framework;

namespace backend.Backend.Tests.Security.Auth;

public sealed class RegisterPolicyTests
{
  [Test]
  public void CanRegisterPentester_ShouldReturnBadRequest_WhenEmailAlreadyExists()
  {
    var policy = new RegisterPolicy();

    var result = policy.CanRegisterPentester(
      true, 
      false, 
      CreatePentesterCommand()
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.User.AlreadyExists));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.BadRequest));
  }
  [Test]
  public void CanRegisterPentester_ShouldReturnBadRequest_WhenNicknameAlreadyExists()
  {
    var policy = new RegisterPolicy();

    var result = policy.CanRegisterPentester(
      false,
      true,
      CreatePentesterCommand()
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.User.AlreadyExists));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.BadRequest));
  }
  [Test]
  public void CanRegisterCompany_ShouldReturnBadRequest_WhenEmailAlreadyExists()
  {
    var policy = new RegisterPolicy();

    var result = policy.CanRegisterCompany(
      true,
      CreateCompanyCommand()
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.User.AlreadyExists));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.BadRequest));
  }

  [Test]
  public void CanRegister_ShouldReturnBadRequest_WhenEmailIsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var command = CreatePentesterCommand(
      email: "testagmail.com"
    );

    var result = policy.CanRegisterPentester(
      false,
      false,
      command
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.Validation.InvalidFormat));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.BadRequest));
  }

  [Test]
  public void CanRegister_ShouldReturnBadRequest_WhenPasswordIsInvalidFormat()
  {
    var policy = new RegisterPolicy();

    var command = CreatePentesterCommand(
      email: "testagmail.com", 
      password: "hereisnoupperletter_123"
    );

    var result = policy.CanRegisterPentester(
      false, 
      false,
      command
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.Validation.InvalidFormat));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.BadRequest));
  }

  [Test]
  public void CanRegister_ShouldReturnBadRequest_WhenPasswordContainsEmail()
  {
    var policy = new RegisterPolicy();

    var command = CreatePentesterCommand(
      email: "test@gmail.com", 
      password: "Test@gmail.com123"
    );

    var result = policy.CanRegisterPentester(
      false,
      false,
      command
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.Validation.InvalidFormat));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.BadRequest));
  }

  [Test]
  public void CanRegister_ShouldReturnBadRequest_WhenPasswordContainsName()
  {
    var policy = new RegisterPolicy();

    var command = CreateCompanyCommand(
      email: "test@gmail.com",
      password: "JanPolSa123__",
      name: "JanPolSa"
    );

    var result = policy.CanRegisterCompany(
      false, 
      command
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.Validation.InvalidFormat));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.BadRequest));
  }

  [Test]
  public void CanRegisterPentester_ShouldReturnSuccess()
  {
    var policy = new RegisterPolicy();

    var command = CreatePentesterCommand();

    var result = policy.CanRegisterPentester(
      false,
      false,
      command
    );

    Assert.That(result.IsSuccess, Is.True);
  }

  [Test]
  public void CanRegisterCompany_ShouldReturnSuccess()
  {
    var policy = new RegisterPolicy();
    var command = CreateCompanyCommand();
    var result = policy.CanRegisterCompany(
      false,
      command
    );
  
    Assert.That(result.IsSuccess, Is.True);
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