using backend.Api.Http;
using backend.Application.Features.Auth.Commands;
using backend.Application.Features.Auth.Dto;
using backend.Domain.Entities;
using backend.Domain.Interfaces.Features;
using Moq;

namespace Backend.Tests.UnitTests.Application.Features.Auth.Commands;

public sealed class RefreshTokenCommandHandlerTests
{
  private readonly Guid UserId = Guid.NewGuid();
  private const string TestIpAddress = "127.0.0.1";
  private const string TestUserAgent = "test-agent";
  private const string ValidRefreshToken = "RefreshToken123";

  private Mock<IAuthSessionService> _mockSession = null!;
  private Mock<ICredentialsService> _mockCreds = null!;
  private RefreshTokenCommandHandler _handler = null!;
  private RefreshTokenCommand _command = null!;

  [SetUp]
  public void SetUp()
  {
    _mockSession = new Mock<IAuthSessionService>();
    _mockCreds = new Mock<ICredentialsService>();
    _handler = new RefreshTokenCommandHandler(
      _mockSession.Object,
      _mockCreds.Object
    );
    _command = new RefreshTokenCommand(
      IpAddress: TestIpAddress,
      UserAgent: TestUserAgent,
      RefreshToken: ValidRefreshToken
    );
  }

  [Test]
  public async Task HandleAsync_ShouldReturnNotFound_WhenTokenIsNullOrEmpty()
  {
    var command = new RefreshTokenCommand(
      IpAddress: TestIpAddress,
      UserAgent: TestUserAgent,
      RefreshToken: null
    );
    var result = await _handler.HandleAsync(
      command, 
      CancellationToken.None
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code ,Is.EqualTo(DomainCodes.Auth.TokenNotFound));
    Assert.That(result.Error.State ,Is.EqualTo(HttpResponseState.NotFound));

    _mockSession.Verify(
      x => x.GetSessionByRefreshTokenAsync(
        It.IsAny<string>(),
        It.IsAny<CancellationToken>()
      ), Times.Never
    );
  }

  [Test]
  public async Task HandleAsync_ShouldReturnSessionNotFound_WhenSessionDoesNotExist()
  {
    _mockSession
      .Setup(x => 
        x.GetSessionByRefreshTokenAsync(
          ValidRefreshToken,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync((Session?)null);

    var result = await _handler.HandleAsync(
      _command,
      CancellationToken.None
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.Auth.SessionNotFound));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.BadRequest));

    _mockSession.Verify(
      x => x.GetSessionByRefreshTokenAsync(
        ValidRefreshToken,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );
    _mockSession.Verify(
      x => x.RevokeAllSessionsAsync(
        UserId, 
        It.IsAny<long?>(), 
        It.IsAny<CancellationToken>()
      ), Times.Never
    );
  }

  [Test]
  public async Task HandleAsync_ShouldReturnTokenTampered_WhenSessionIsRevokedOrUsed()
  {
    var session = Session.Create(
      UserId,
      ValidRefreshToken,
      TestUserAgent,
      TestIpAddress
    );
    session.RevokeSession(null);

    _mockSession
      .Setup(x => 
        x.GetSessionByRefreshTokenAsync(
          ValidRefreshToken,
          It.IsAny<CancellationToken>()
        )
      )
      .ReturnsAsync(session);

    var result = await _handler.HandleAsync(
      _command,
      CancellationToken.None
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code, Is.EqualTo(DomainCodes.Auth.TokenTampered));
    Assert.That(result.Error.State, Is.EqualTo(HttpResponseState.Forbidden));

    _mockSession.Verify(
      x => x.GetSessionByRefreshTokenAsync(
        ValidRefreshToken,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );
    _mockSession.Verify(
      x => x.RevokeAllSessionsAsync(
        UserId,
        null,
        It.IsAny<CancellationToken>()
      ), Times.Once
    );
    _mockSession.Verify(
      x => x.RevokeSessionByIdAsync(
        UserId,
        session.Id,
        It.IsAny<long?>(),
        It.IsAny<CancellationToken>()
      ), Times.Never
    );
  }

  [Test]
  public async Task HandleAsync_ShouldReturnSuccess_WhenTokenAndSessionExists()
  {
    var oldSession = Session.Create(
      userId: UserId,
      refreshToken: ValidRefreshToken,
      userAgent: TestUserAgent,
      ipAddress: TestIpAddress
    );

    var tokens = new CredentialsDto(
      AuthToken: "NewAuthToken",
      RefreshToken: "NewRefreshToken"
    );

    var newSession = Session.Create(
      userId: UserId,
      refreshToken: tokens.RefreshToken,
      userAgent: TestUserAgent,
      ipAddress: TestIpAddress
    );

    _mockSession
      .Setup(x => 
        x.GetSessionByRefreshTokenAsync(
          ValidRefreshToken,
          It.IsAny<CancellationToken>()
        ))
      .ReturnsAsync(oldSession);

      _mockSession
        .Setup(x => 
          x.TryUseRefreshTokenAsync(
            oldSession.Id,
            It.IsAny<CancellationToken>()
          ))
        .ReturnsAsync(true);
      
      _mockCreds
        .Setup(x => 
          x.GenerateCredentials(
            UserId,
            It.IsAny<CancellationToken>()
          ))
        .ReturnsAsync(tokens);

      _mockSession
        .Setup(x => 
          x.CreateSessionAsync(
            UserId,
            tokens.RefreshToken,
            It.IsAny<string>(),
            TestUserAgent,
            It.IsAny<CancellationToken>()
          ))
        .ReturnsAsync(newSession);

      


      var result = await _handler.HandleAsync(
        _command,
        CancellationToken.None
      );

      Assert.That(result.IsSuccess, Is.True);
      Assert.That(result.Value, Is.EqualTo(tokens));

      _mockSession.Verify(
        x => x.GetSessionByRefreshTokenAsync(
          ValidRefreshToken,
          It.IsAny<CancellationToken>()
        ), Times.Once
      );

      _mockSession.Verify(
        x => x.TryUseRefreshTokenAsync(
          oldSession.Id,
          It.IsAny<CancellationToken>()
        ), Times.Once
      );

      _mockSession.Verify(
        x => x.CreateSessionAsync(
          UserId,
          tokens.RefreshToken,
          It.IsAny<string>(),
          TestUserAgent,
          It.IsAny<CancellationToken>()
        ), Times.Once
      );

      _mockSession.Verify(
        x => x.SetReplacedByAndRevokedAsync(
          oldSession.Id,
          newSession.Id,
          It.IsAny<CancellationToken>()
        ), Times.Once
      );

      _mockCreds.Verify(
        x => x.GenerateCredentials(
          UserId,
          It.IsAny<CancellationToken>()
        ), Times.Once
      );

  }
}