using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Dto;
using backend.Domain.Interfaces.Features;

namespace backend.Application.Features.Auth.Commands;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, CredentialsDto>
{

  private readonly IAuthSessionService _sessionService;
  private readonly ICredentialsService _credsService;
  public RefreshTokenCommandHandler(
    IAuthSessionService sessionService,
    ICredentialsService credentialsService
  )
  {
    _sessionService = sessionService;
    _credsService = credentialsService;
  }

  public async Task<Result<CredentialsDto>> HandleAsync(RefreshTokenCommand req, CancellationToken ct)
  {
    if (string.IsNullOrEmpty(req.RefreshToken))
      return Result<CredentialsDto>.Failure(new Error(
        DomainCodes.Auth.TokenNotFound,
        "Brak tokenu",
        HttpResponseState.NotFound
      ));

    var validationResult = await ValidateAndHandleRefreshTokenAsync(req.RefreshToken, ct);
    if(validationResult.IsFailure)
      return Result<CredentialsDto>.Failure(validationResult.Error);

    var session = validationResult.Value;

    var used = await _sessionService.TryUseRefreshTokenAsync(session.Session.Id, ct);
    if (!used)
    {
      await _sessionService.RevokeAllSessionsAsync(session.UserId, session.Session.Id, ct);
      return Result<CredentialsDto>.Failure(new Error(
        DomainCodes.Auth.TokenTampered,
        "Token został sfałszowany",
        HttpResponseState.Forbidden
      ));
    }

    var tokens = await _credsService.GenerateCredentials(session.UserId, ct);
    var newSession = await _sessionService.CreateSessionAsync(session.UserId, tokens.RefreshToken, req.IpAddress, req.UserAgent, ct);

    await _sessionService.SetReplacedByAndRevokedAsync(session.Session.Id, newSession.Id, ct);

    return tokens;
  }

  private async Task<Result<ValidatedSession>> ValidateAndHandleRefreshTokenAsync(string refreshToken, CancellationToken ct)
  {
    var session = await _sessionService.GetSessionByRefreshTokenAsync(refreshToken, ct);
    if(session is null)
      return Result<ValidatedSession>.Failure(new Error(
        DomainCodes.Auth.SessionNotFound,
        "Session not found",
        HttpResponseState.BadRequest
      ));

    if (session.Revoked || session.Used)
    {
      await _sessionService.RevokeAllSessionsAsync(session.UserId, null, ct);
      return Result<ValidatedSession>.Failure(new Error(
        DomainCodes.Auth.TokenTampered,
        "Użyty token ma flagę: 'used' oraz 'revoked'",
        HttpResponseState.Forbidden
      ));
    }

    if (session.ExpiresAt <= DateTime.UtcNow)
    {
      await _sessionService.RevokeSessionByIdAsync(session.UserId, session.Id, null, ct);
      return Result<ValidatedSession>.Failure(new Error(
        DomainCodes.Auth.TokenExpired,
        "Token Expired",
        HttpResponseState.Forbidden
      ));
    }

    return new ValidatedSession(
      session.UserId,
      session
    );
  }
}