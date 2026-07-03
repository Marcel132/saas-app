using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Entities.Records;
using backend.Domain.Policies;
using NUnit.Framework;

namespace backend.Backend.Tests.Security.Auth;

public class LoginPolicyTests
{
  [Test]
  public void Validate_UserIsNull_ThrowsInvalidCredentials()
  {
    var policy = new LoginPolicy();

    Assert.Throws<InvalidCredentialsAppException>(() =>
      policy.EnsureCanLogin(null)
    );
  }

  [Test]
  public void Validate_BlockDuration_ThrowsInvalidCredentials()
  {
    var policy = new LoginPolicy();

    var user = CreateUser();

    user.RegisterFailedLoginAttempt(1, TimeSpan.FromMinutes(15));

    Assert.Throws<InvalidCredentialsAppException>(() =>
      policy.EnsureCanLogin(user)
    );
  }

  [Test]
  public void Validate_UserIsNotActive_ThrowsInvalidCredentials()
  {
    var policy = new LoginPolicy();

    var user = CreateUser();

    user.DeactivateAccount();

    Assert.Throws<InvalidCredentialsAppException>(() =>
      policy.EnsureCanLogin(user)
    );
  }

  [Test]
  public void Validate_UserIsValid_Success()
  {
    var policy = new LoginPolicy();

    var user = CreateUser();

    Assert.DoesNotThrow(() =>
    {
      policy.EnsureCanLogin(user);
    });

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