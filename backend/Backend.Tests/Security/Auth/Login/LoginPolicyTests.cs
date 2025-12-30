using NUnit.Framework;

public class LoginPolicyTests
{
    [Test]
    public void Validate_UserIsNull_ThrowsUnauthorized()
    {
        var policy = new LoginPolicy();

        Assert.Throws<InvalidCredentialsAppException>(() => 
            policy.EnsureCanLogin(null, "")
        );
    }

    [Test]
    public void Validate_PasswordIsInvalid_ThrowsUnauthorized()
    {
        var policy = new LoginPolicy();

        var user = new UserLoginDataDto
        {
            HashedPassword = BCrypt.Net.BCrypt.HashPassword("test123"),
            IsActive = true
        };

        Assert.Throws<InvalidCredentialsAppException>(() => 
            policy.EnsureCanLogin(user, "321test")
        );
    }

    [Test]
    public void Validate_UserIsNotActive_ThrowsForbidden()
    {
        var policy = new LoginPolicy();

        var user = new UserLoginDataDto
        {
            HashedPassword = BCrypt.Net.BCrypt.HashPassword("test"),
            IsActive = false
        };

        Assert.Throws<ForbiddenAppException>(() => 
            policy.EnsureCanLogin(user, "test")
        );
    }

    [Test]
    public void Validate_UserIsValid_Success()
    {
        var policy = new LoginPolicy();

        var user = new UserLoginDataDto
        {
            HashedPassword = BCrypt.Net.BCrypt.HashPassword("test"),
            IsActive = true,
        };

        Assert.DoesNotThrow(() => {
            policy.EnsureCanLogin(user, "test");
        });
    }
}