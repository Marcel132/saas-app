using backend.Api.Http;
using backend.Application.Features.Auth.Commands;
using NUnit.Framework;

namespace backend.Backend.Tests.UnitTests.Application.Features.Auth.Commands;

public sealed class RefreshTokenCommandHandlerTests
{
  [Test]
  public async Task HandleAsync_ShouldReturnNotFound_WhenTokenIsNullOrEmpty()
  {
    var command = new RefreshTokenCommand(
      IpAddress: "",
      UserAgent: "",
      RefreshToken: ""
    );

    var handler = new RefreshTokenCommandHandler(
      null!,
      null!
    );

    var result = await handler.HandleAsync(
      command, 
      CancellationToken.None
    );

    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error.Code ,Is.EqualTo(DomainCodes.Auth.TokenNotFound));
    Assert.That(result.Error.State ,Is.EqualTo(HttpResponseState.NotFound));
  }
}