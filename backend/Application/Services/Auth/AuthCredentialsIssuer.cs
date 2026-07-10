using backend.Application.Services.Auth.DTOs;
using backend.Domain.Interfaces.Features;

namespace backend.Application.Services.Auth;

public class AuthCredentialsIssuer
{
  private readonly AuthSessionService _sessionService;
  private readonly ICredentialsService _credentialsService;

  public AuthCredentialsIssuer(AuthSessionService sessionService, ICredentialsService credentialsService)
  {
    _sessionService = sessionService;
    _credentialsService = credentialsService;
  }

  public async Task<CredentialsDto> IssueAsync(Guid userId, string ipAddress, string userAgent)
  {
    var tokens = await _credentialsService.GenerateCredentials(userId);

    await _sessionService.RevokeAllSessionsAsync(userId, null);
    await _sessionService.CreateSessionAsync(userId, tokens.RefreshToken, ipAddress, userAgent);

    return tokens;
  }
}