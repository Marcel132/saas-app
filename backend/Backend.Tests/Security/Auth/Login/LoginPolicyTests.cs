using backend.Api.Http;
using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Entities.Records;
using backend.Domain.Policies;
using NUnit.Framework;

namespace backend.Backend.Tests.Security.Auth;

public sealed class LoginPolicyTests
{
  [Test]
  public void CanLogin_ShouldReturnUnauthorized_WhenUserIsNull() 
  {
    var policy = new LoginPolicy();

    var result = policy.CanLogin(null);

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.Auth.InvalidCredentials));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.Unauthorized));
  }

  [Test]
  public void CanLogin_ShouldReturnUnauthorized_WhenAccountIsDeactivated()
  {
    var policy = new LoginPolicy();

    var user = CreateUser();

    user.DeactivateAccount();

    var result = policy.CanLogin(user);

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.Auth.InvalidCredentials));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.Unauthorized));
  }

  [Test]
  public void CanLogin_ShouldReturnUnauthorized_WhenUserReachedMaxLoginAttempts()
  {
    var policy = new LoginPolicy();

    var user = CreateUser();

    user.RegisterFailedLoginAttempt(1, TimeSpan.FromMinutes(15));

    var result = policy.CanLogin(user);

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.Auth.InvalidCredentials));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.Unauthorized));
  }


  [Test]
  public void CanLogin_ShouldReturnSuccess()
  {
    var policy = new LoginPolicy();

    var user = CreateUser();

    var result = policy.CanLogin(user);

    Assert.That(result.IsSuccess, Is.True);
  }

  private static User CreateUser()
  {
    var record = new UserRecord(
      "text@gmail.com",
      "test_1234",
      RoleType.Pentester
    );

    return new User(record);
  }
}