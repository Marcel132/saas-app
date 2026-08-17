using backend.Api.Http;
using backend.Application.Features.Users.Commands;
using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Entities.Records;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Features;
using backend.Domain.Interfaces.Repositories;
using Moq;

namespace Backend.Tests.UnitTests.Application.Features.Users.Commands;

public sealed class DeleteUserCommandHandlerTests
{
  private readonly Guid UserId = Guid.NewGuid();
  private Mock<IUserRepository> _mockRepo = null!;
  private Mock<IAuthSessionService> _mockSession = null!;
  private Mock<IUnitOfWork> _mockUnitOfWork = null!;
  private DeleteUserCommandHandler _handler = null!;
  private DeleteUserCommand _command = null!;

  [SetUp]
  public void SetUp()
  {
    _mockRepo = new Mock<IUserRepository>();
    _mockUnitOfWork = new Mock<IUnitOfWork>();
    _mockSession = new Mock<IAuthSessionService>();

    _handler = new DeleteUserCommandHandler(
      _mockRepo.Object,
      _mockUnitOfWork.Object,
      _mockSession.Object
    );

    _command = new DeleteUserCommand(
      UserId: UserId
    );
  }


  [Test]
  public async Task HandleAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
  {
    _mockRepo
      .Setup(x => 
        x.GetByIdAsync(
          _command.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync((User?)null);

    var result = await _handler.HandleAsync(
      _command, 
      CancellationToken.None
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.User.NotFound));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.NotFound));

    _mockRepo.Verify(
      x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
      Times.Once);

    _mockUnitOfWork.Verify(
      x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
      Times.Never);
    
    _mockSession.Verify(
      x => x.RevokeAllSessionsAsync(
        UserId,
        It.IsAny<long?>(),
        It.IsAny<CancellationToken>()
      ), Times.Never
    );
  }

  [Test]
  public async Task HandleAsync_ShouldReturnSuccess_WhenUserExists()
  {
    var userRecord = new UserRecord(
      "testtest1@gmail.com123123123",
      "Password123!",
      RoleType.Pentester
    );

    var user = new User(
      userRecord
    );
    var command = new DeleteUserCommand(
      user.Id
    );

    _mockRepo
      .Setup(x => 
        x.GetByIdAsync(
          command.UserId, 
          It.IsAny<CancellationToken>()
        ) 
      )
      .ReturnsAsync(user);

    var result = await _handler.HandleAsync(
      command, 
      CancellationToken.None
    );

    Assert.That(result.IsSuccess, Is.True);
    Assert.That(user.IsActive, Is.False);

    _mockRepo.Verify(
      x => x.GetByIdAsync(
        It.IsAny<Guid>(), 
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    _mockUnitOfWork.Verify(
      x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
      Times.Once);
    
    _mockSession.Verify(
      x => x.RevokeAllSessionsAsync(
        command.UserId,
        It.IsAny<long?>(),
        It.IsAny<CancellationToken>()
      ), Times.Once
    );
  }
}