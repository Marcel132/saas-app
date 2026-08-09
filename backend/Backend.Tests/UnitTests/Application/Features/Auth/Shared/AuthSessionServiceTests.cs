using backend.Application.Features.Auth.Shared;
using backend.Domain.Entities;
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

  [Test]
  public async Task RevokeAllSessionsAsync_ShouldRevokeSession_AndSaveChanges()
  {
    var mockQueryRepo = new Mock<ISessionQueryRepository>();
    var mockRepo = new Mock<ISessionRepository>();
    var mockUow = new Mock<IUnitOfWork>();

    var userId = Guid.NewGuid();
    var session1 = Session.Create(
      userId,
      "RefreshToken1",
      "",
      ""
    );
    var session2 = Session.Create(
      userId,
      "RefreshToken2",
      "",
      ""
    );

    mockQueryRepo
      .Setup(x =>
        x.GetAllActiveSessionsAsync(
          userId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(
        [
        session1,
        session2
        ]
      );

    var service = new AuthSessionService(
      mockRepo.Object,
      mockQueryRepo.Object,
      mockUow.Object
    );

    await service.RevokeAllSessionsAsync(
      userId,
      null,
      CancellationToken.None
    );

    Assert.That(session1.Revoked, Is.True);
    Assert.That(session2.Revoked, Is.True);

    mockQueryRepo.Verify(
      x => x.GetAllActiveSessionsAsync(
        userId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockRepo.Verify(
      x => x.Update(session1), Times.Once
    );
    mockRepo.Verify(
      x => x.Update(session2), Times.Once
    );

    mockUow.Verify(
      x => x.SaveChangesAsync(
        It.IsAny<CancellationToken>()
      ), Times.Once
    );
  }

  [Test]
  public async Task RevokeAllSessionsAsync_ShouldNotUpdateOrSave_WhenNoActiveSessionsExist()
  {
    var mockQueryRepo = new Mock<ISessionQueryRepository>();
    var mockRepo = new Mock<ISessionRepository>();
    var mockUow = new Mock<IUnitOfWork>();

    var userId = Guid.NewGuid();

    mockQueryRepo
      .Setup(x =>
        x.GetAllActiveSessionsAsync(
          userId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync([]);

    var service = new AuthSessionService(
      mockRepo.Object,
      mockQueryRepo.Object,
      mockUow.Object
    );

    await service.RevokeAllSessionsAsync(
      userId,
      null,
      CancellationToken.None
    );

    mockQueryRepo.Verify(
      x => x.GetAllActiveSessionsAsync(
        userId,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockRepo.Verify(
      x => x.Update(It.IsAny<Session>()), Times.Never
    );
    mockUow.Verify(
      x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never
    );

  }

  [Test]
  public async Task RevokeSessionByIdAsync_ShouldRevokeSession_AndSaveChanges()
  {
    var mockQueryRepo = new Mock<ISessionQueryRepository>();
    var mockRepo = new Mock<ISessionRepository>();
    var mockUow = new Mock<IUnitOfWork>();

    var userId = Guid.NewGuid();
    var session = Session.Create(
      userId,
      "RefreshToken1",
      "",
      ""
    );

    mockQueryRepo
      .Setup(x =>
        x.GetSessionByUserAndIdAsync(
          userId,
          session.Id,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(session);

    var service = new AuthSessionService(
      mockRepo.Object,
      mockQueryRepo.Object,
      mockUow.Object
    );

    await service.RevokeSessionByIdAsync(
      userId,
      session.Id,
      3,
      CancellationToken.None
    );

    Assert.That(session.ReplacedByTokenId, Is.EqualTo(3));

    mockRepo.Verify(
      x => x.Update(It.IsAny<Session>()), Times.Once
    );
    mockQueryRepo.Verify(
      x => x.GetSessionByUserAndIdAsync(
        userId,
        session.Id,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockUow.Verify(
      x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once
    );
  }

  [Test]
  public void RevokeSessionByIdAsync_ShouldThrowSessionNotFoundAppException_WhenSessionDoesNotExist()
  {
    var mockQueryRepo = new Mock<ISessionQueryRepository>();

    long sessionId = 1234;
    var userId = Guid.NewGuid();

    mockQueryRepo
      .Setup(x =>
        x.GetSessionByUserAndIdAsync(
          userId,
          sessionId,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync((Session?)null);

    var service = new AuthSessionService(
      null!,
      mockQueryRepo.Object,
      null!
    );


    Assert.ThrowsAsync<SessionNotFoundAppException>(async () =>
      await service.RevokeSessionByIdAsync(
        userId,
        sessionId,
        null,
        CancellationToken.None
      )
    );
  }

  [Test]
  public async Task RevokeActiveSessionAsync_ShouldNotUpdateOrSave_WhenNoActiveSessionExist()
  {
    var mockQueryRepo = new Mock<ISessionQueryRepository>();
    var mockRepo = new Mock<ISessionRepository>();
    var mockUow = new Mock<IUnitOfWork>();

    var refreshToken = "RefreshToken123";

    mockQueryRepo 
      .Setup(x => 
        x.GetSessionByRefreshTokenAsync(
          refreshToken,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync((Session?)null);

    var service = new AuthSessionService(
      mockRepo.Object,
      mockQueryRepo.Object,
      mockUow.Object
    );

    await service.RevokeActiveSessionAsync(
      refreshToken,
      null,
      CancellationToken.None
    );
  
    mockQueryRepo.Verify(
      x => x.GetSessionByRefreshTokenAsync(
        refreshToken,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );

    mockRepo.Verify(
      x => x.Update(It.IsAny<Session>()), Times.Never
    );

    mockUow.Verify(
      x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never
    );
  }

  [Test]
  public async Task RevokeActiveSessionAsync_ShouldRevokeSession_AndSaveChanges()
  {
    var mockQueryRepo = new Mock<ISessionQueryRepository>();
    var mockRepo = new Mock<ISessionRepository>();
    var mockUow = new Mock<IUnitOfWork>();

    var userId = Guid.NewGuid();
    var refreshToken = "RefreshToken123";
    var session = Session.Create(
      userId,
      refreshToken,
      "",
      ""
    );

    mockQueryRepo
      .Setup(x => 
        x.GetSessionByRefreshTokenAsync(
          refreshToken,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(session);

    var service = new AuthSessionService(
      mockRepo.Object,
      mockQueryRepo.Object,
      mockUow.Object
    );

    await service.RevokeActiveSessionAsync(
      refreshToken,
      3,
      CancellationToken.None
    );

    Assert.That(session.Revoked, Is.True);
    Assert.That(session.Used, Is.True);
    Assert.That(session.ReplacedByTokenId, Is.EqualTo(3));

    mockRepo.Verify(
      x => x.Update(session), Times.Once
    );

    mockQueryRepo.Verify(
      x => x.GetSessionByRefreshTokenAsync(
        refreshToken,
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