using backend.Api.Http;
using backend.Application.Features.Users.Commands;
using backend.Domain.Entities;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;
using Moq;
using NUnit.Framework;

namespace backend.Backend.Tests.UnitTests.Application.Features.Users.Commands;

public sealed class DeleteUserCommandHandlerTests
{
  [Test]
  public async Task HandleAsync_ShouldReturnNotFound_WhenUserDoesNotExists()
  {
    var command = new DeleteUserCommand(
      Guid.NewGuid()
    );
    var userRepository = new Mock<IUserRepository>();

    userRepository
      .Setup(x => 
        x.GetByIdAsync(
          command.UserId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync((User?)null);

    var unitOfWork = new Mock<IUnitOfWork>();

    var handler = new DeleteUserCommandHandler(
      userRepository.Object,
      unitOfWork.Object
    );

    var result = await handler.HandleAsync(command, CancellationToken.None);

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.User.NotFound));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.NotFound));

    userRepository.Verify(
      x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
      Times.Once);

    unitOfWork.Verify(
      x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
      Times.Never);
  }
}