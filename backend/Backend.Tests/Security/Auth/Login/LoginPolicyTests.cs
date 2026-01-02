using NUnit.Framework;

public class LoginPolicyTests
{
    [Test]
    public void Validate_UserIsNull_ThrowsUnauthorized()
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
      var user = new User(
        "test@testmail.com",
        "test123"
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

        var user = new User(
          "test@testmail.com",
          "test123"
        );

        user.Deactivate();

        Assert.Throws<ForbiddenAppException>(() => 
            policy.EnsureCanLogin(user)
        );
    }

    [Test]
    public void Validate_UserIsValid_Success()
    {
        var policy = new LoginPolicy();

        var user = new User(
          "test@testmail.com",
          "test123"
        );

        Assert.DoesNotThrow(() => {
            policy.EnsureCanLogin(user);
        });
    }
}