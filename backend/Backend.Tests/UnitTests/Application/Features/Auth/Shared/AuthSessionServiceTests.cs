using backend.Application.Features.Auth.Shared;
using backend.Domain.Interfaces;
using backend.Domain.Interfaces.Repositories;
using Moq;
using NUnit.Framework;

namespace backend.Backend.Tests.UnitTests.Application.Features.Auth.Shared;

public sealed class AuthSessionServiceTests
{
  [Test]
  public async Task CreateSessionAsync_ShouldPersistSession_AndReturnIt()
  {
    var mockRepo = new Mock<ISessionRepository>();
    var mockQueryRepo = new Mock<ISessionQueryRepository>();
    var mockUow = new Mock<IUnitOfWork>();

    var service = new AuthSessionService(
      mockRepo.Object,
      mockQueryRepo.Object,
      mockUow.Object
    );

    var userId = Guid.NewGuid();

    var session = await service.CreateSessionAsync(
      userId: userId,
      refreshToken: "RefreshToken",
      deviceIp: "",
      userAgent: "",
      ct: CancellationToken.None
    );

    Assert.That(session, Is.Not.Null);
    Assert.That(session.UserId, Is.EqualTo(userId));

    mockRepo.Verify(
      x => x.AddAsync(
        session,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockUow.Verify(
      x => x.SaveChangesAsync(
        It.IsAny<CancellationToken>()
      ), Times.Once
    );
  }
}