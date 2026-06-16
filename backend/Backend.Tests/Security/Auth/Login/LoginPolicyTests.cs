using backend.Api.Controllers.Auth.DTOs;
using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
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
  public void Validate_BlockDuration_ThrowsAccountBlocked()
  {
    var policy = new LoginPolicy();

    var data = new RegisterRequestDto()
    {
      Email = "test@testmail.com",
      Password = "test123",
      Role = RoleType.Pentester,
      FirstName = "Adam",
      LastName = "Nowak",
      Nickname = "AdamNowak",
      PhoneNumber = "123",
      Description = "Skills",
      City = "City",
      Country = "Country",
      PostalCode = "00-000",
      Street = "Street"
    };

    var userData = new UserData(data);
    var user = new User(
      "test@testmail.com",
      "test123",
      data.Role,
      userData
    );

    user.RegisterFailedLoginAttempt(1, TimeSpan.FromMinutes(15));

    Assert.Throws<AccountBlockedAppException>(() =>
      policy.EnsureCanLogin(user)
    );
  }

  [Test]
  public void Validate_UserIsNotActive_ThrowsForbidden()
  {
    var policy = new LoginPolicy();

    var data = new RegisterRequestDto()
    {
      Email = "test@testmail.com",
      Password = "test123",
      Role = RoleType.Pentester,
      FirstName = "Adam",
      LastName = "Nowak",
      Nickname = "AdamNowak",
      PhoneNumber = "123",
      Description = "Skills",
      City = "City",
      Country = "Country",
      PostalCode = "00-000",
      Street = "Street"
    };

    var userData = new UserData(data);

    var user = new User(
      "test@testmail.com",
      "test123",
      data.Role,
      userData
    );

    user.DeactivateAccount();

    Assert.Throws<ForbiddenAppException>(() =>
      policy.EnsureCanLogin(user)
    );
  }

  [Test]
  public void Validate_UserIsValid_Success()
  {
    var policy = new LoginPolicy();

    var data = new RegisterRequestDto()
    {
      Email = "test@testmail.com",
      Password = "test123",
      Role = RoleType.Pentester,
      FirstName = "Adam",
      LastName = "Nowak",
      Nickname = "AdamNowak",
      PhoneNumber = "123",
      Description = "Skills",
      City = "City",
      Country = "Country",
      PostalCode = "00-000",
      Street = "Street"
    };

    var userData = new UserData(data);

    var user = new User(
      "test@testmail.com",
      "test123",
      data.Role,
      userData
    );

    Assert.DoesNotThrow(() =>
    {
      policy.EnsureCanLogin(user);
    });
  }
}