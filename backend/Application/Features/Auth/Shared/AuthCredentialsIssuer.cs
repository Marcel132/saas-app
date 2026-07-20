using backend.Application.Features.Auth.Dto;
using backend.Domain.Interfaces.Features;

namespace backend.Application.Features.Auth.Shared;

public class AuthCredentialsIssuer
{
  private readonly AuthSessionService _sessionService;
  private readonly ICredentialsService _credentialsService;

  public AuthCredentialsIssuer(AuthSessionService sessionService, ICredentialsService credentialsService)
  {
    _sessionService = sessionService;
    _credentialsService = credentialsService;
  }

  public async Task<CredentialsDto> IssueAsync(Guid userId, string ipAddress, string userAgent , CancellationToken ct)
  {
    var tokens = await _credentialsService.GenerateCredentials(userId, ct);

    await _sessionService.RevokeAllSessionsAsync(userId, null, ct);
    await _sessionService.CreateSessionAsync(userId, tokens.RefreshToken, ipAddress, userAgent, ct);

    return tokens;
  }
}