using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Commands;
using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Entities.Records;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;
using Moq;
using NUnit.Framework;

namespace backend.Backend.Tests.UnitTests.Application.Features.Auth.Commands;

public sealed class UserAuthenticationServiceTests
{
  [Test]
  public async Task AuthenticateAsync_ShouldReturnInvalidCredentials_WhenUserDoesNotExist()
  {
    var command = new LoginCommand(
      Email: "test@example.com",
      Password: "TestPassword.com",
      IpAddress: "",
      UserAgent: ""
    );

    var mockUsers = new Mock<IUserRepository>();
    var mockHasher = new Mock<IPasswordHasher>();
    var mockPolicy = new Mock<ILoginPolicy>();
    var mockUow = new Mock<IUnitOfWork>();


    mockUsers
      .Setup(x =>
        x.GetByEmailAsync(
          command.Email,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync((User?)null);


    var service = new UserAuthenticationService(
      mockUsers.Object,
      mockHasher.Object,
      mockPolicy.Object,
      mockUow.Object
    );

    var result = await service.AuthenticateAsync(command, CancellationToken.None);

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.Auth.InvalidCredentials));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.BadRequest));

    mockUsers.Verify(
      x => x.GetByEmailAsync(
        command.Email,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );
    mockHasher.Verify(
      x => x.Verify(
        It.IsAny<string>(),
        It.IsAny<string>()
      ), Times.Never
    );
  }

  [Test]
  public async Task AuthenticateAsync_ShouldReturnPolicyError_WhenPolicyResultIsFailure()
  {
    var command = new LoginCommand(
      Email: "test@example.com",
      Password: "TestPassword123!",
      IpAddress: "",
      UserAgent: ""
    );

    var mockUsers = new Mock<IUserRepository>();
    var mockHasher = new Mock<IPasswordHasher>();
    var mockPolicy = new Mock<ILoginPolicy>();
    var mockUow = new Mock<IUnitOfWork>();

    var user = new User(
      new UserRecord(
        NormalizedEmail: "test@gmail.com",
        PasswordHash: "TestPassword123!",
        Role: RoleType.Pentester
      )
    );

    var policyError = new Error(
      DomainCodes.Auth.InvalidCredentials,
      "Nieprawidłowy email lub hasło",
      HttpResponseState.Unauthorized
    );

    mockUsers
      .Setup(x =>
        x.GetByEmailAsync(
          command.Email,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(user);

    mockPolicy
      .Setup(
        x => x.CanLogin(
          user
        )
      )
      .Returns(Result.Failure(policyError));

    var service = new UserAuthenticationService(
      mockUsers.Object,
      mockHasher.Object,
      mockPolicy.Object,
      mockUow.Object
    );

    var result = await service.AuthenticateAsync(
      command,
      CancellationToken.None
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(policyError.Code));

    mockHasher.Verify(
      x => x.Verify(
        It.IsAny<string>(),
        It.IsAny<string>()
      ), Times.Never
    );
    mockUow.Verify(
      x => x.SaveChangesAsync(
        It.IsAny<CancellationToken>()
      ), Times.Never
    );
  }

  [Test]
  public async Task AuthenticateAsync_ShouldReturnInvalidCredentials_WhenPasswordIsIncorrect()
  {
    var mockRepo = new Mock<IUserRepository>();
    var mockUow = new Mock<IUnitOfWork>();
    var mockPolicy = new Mock<ILoginPolicy>();
    var mockHasher = new Mock<IPasswordHasher>();

    var command = new LoginCommand(
      Email: "test@example.com",
      Password: "Test_Password123",
      IpAddress: "",
      UserAgent: ""
    );

    var user = new User(
      new UserRecord(
        NormalizedEmail: "test@example.com",
        PasswordHash: "254cd63ece8595b5c503783d596803f1552e0733d02fe4080b217eadb17711dd",
        Role: RoleType.Pentester
      )
    );

    mockRepo
      .Setup(x =>
        x.GetByEmailAsync(
          command.Email,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(user);

    mockPolicy
      .Setup(x =>
        x.CanLogin(
          user
        )
      )
      .Returns(Result.Success());

    mockHasher
      .Setup(x =>
        x.Verify(
          command.Password,
          user.PasswordHash
        )
      )
      .Returns(false);

    var service = new UserAuthenticationService(
      mockRepo.Object,
      mockHasher.Object,
      mockPolicy.Object,
      mockUow.Object
    );

    var result = await service.AuthenticateAsync(
      command,
      CancellationToken.None
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.Auth.InvalidCredentials));
    Assert.That(user.FailedLoginAttempts, Is.EqualTo(1));

    mockRepo.Verify(
      x => x.GetByEmailAsync(
        command.Email,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockPolicy.Verify(
      x => x.CanLogin(
        user
      ), Times.Once
    );

    mockHasher.Verify(
      x => x.Verify(
        command.Password,
        user.PasswordHash
      ), Times.Once
    );

    mockUow.Verify(
      x => x.SaveChangesAsync(
        It.IsAny<CancellationToken>()
      ), Times.Once
    );
  }

  [Test]
  public async Task AuthenticateAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
  {
    var mockRepo = new Mock<IUserRepository>();
    var mockPolicy = new Mock<ILoginPolicy>();
    var mockHasher = new Mock<IPasswordHasher>();
    var mockUow = new Mock<IUnitOfWork>();

    var command = new LoginCommand(
      Email: "test@example.com",
      Password: "Test_Password123",
      IpAddress: "",
      UserAgent: ""
    );

    var user = new User(
      new UserRecord(
        NormalizedEmail: "test@example.com",
        PasswordHash: "254cd63ece8595b5c503783d596803f1552e0733d02fe4080b217eadb17711dd",
        Role: RoleType.Pentester
      )
    );

    mockRepo
      .Setup(x =>
        x.GetByEmailAsync(
          command.Email,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(user);

    mockPolicy
      .Setup(x =>
        x.CanLogin(user)
      )
      .Returns(Result.Success());

    mockHasher
      .Setup(x =>
        x.Verify(
          command.Password,
          user.PasswordHash
        )
      )
      .Returns(true);

    var service = new UserAuthenticationService(
      mockRepo.Object,
      mockHasher.Object,
      mockPolicy.Object,
      mockUow.Object
    );

    var result = await service.AuthenticateAsync(
      command,
      CancellationToken.None
    );


    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Value, Is.SameAs(user));
    Assert.That(user.FailedLoginAttempts, Is.EqualTo(0));


    mockRepo.Verify(
      x => x.GetByEmailAsync(
        command.Email,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockHasher.Verify(
      x => x.Verify(
        command.Password,
        user.PasswordHash
      ), Times.Once
    );

    mockPolicy.Verify(
      x => x.CanLogin(
        user
      ), Times.Once
    );

    mockUow.Verify(
      x => x.SaveChangesAsync(
        It.IsAny<CancellationToken>()
      ), Times.Once
    );
  }
}