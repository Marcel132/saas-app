using backend.Api.Http;
using backend.Application.Features.Users.Commands;
using backend.Domain.Entities;
using backend.Domain.Entities.Enum;
using backend.Domain.Entities.Records;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;
using Moq;
using NUnit.Framework;

namespace backend.Backend.Tests.UnitTests.Application.Features.Users.Commands;

public sealed class DeleteUserCommandHandlerTests
{
  [Test]
  public async Task HandleAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
  {
    var command = new DeleteUserCommand(
      Guid.NewGuid()
    );
    var mockRepo = new Mock<IUserRepository>();

    mockRepo
      .Setup(x => 
        x.GetByIdAsync(
          command.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync((User?)null);

    var unitOfWork = new Mock<IUnitOfWork>();

    var handler = new DeleteUserCommandHandler(
      mockRepo.Object,
      unitOfWork.Object
    );

    var result = await handler.HandleAsync(command, CancellationToken.None);

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.User.NotFound));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.NotFound));

    mockRepo.Verify(
      x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
      Times.Once);

    unitOfWork.Verify(
      x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
      Times.Never);
  }

  [Test]
  public async Task HandleAsync_ShouldReturnSuccess_WhenUserExists()
  {
    var mockRepo = new Mock<IUserRepository>();
    var unitOfWork = new Mock<IUnitOfWork>();

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
    mockRepo
      .Setup(x => 
        x.GetByIdAsync(
          command.UserId, 
          It.IsAny<CancellationToken>()
        ) 
      )
      .ReturnsAsync(user);
    

    var handler = new DeleteUserCommandHandler(
      mockRepo.Object,
      unitOfWork.Object
    );

    var result = await handler.HandleAsync(command, CancellationToken.None);

    Assert.That(result.IsSuccess, Is.True);
    Assert.That(user.IsActive, Is.False);

    mockRepo.Verify(
      x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
      Times.Once);

    unitOfWork.Verify(
      x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
      Times.Once);
  }
}