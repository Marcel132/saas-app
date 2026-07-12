using backend.Application.Abstractions.CQRS;
using backend.Application.Features.Auth.Shared;

namespace backend.Application.Features.Auth.Commands;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
  private readonly AuthSessionService _sessionService;
  public LogoutCommandHandler(
    AuthSessionService authSessionService
  )
  {
    _sessionService = authSessionService;
  }

  public async Task<Result> HandleAsync(LogoutCommand command, CancellationToken ct)
  {
    await _sessionService.RevokeAllSessionsAsync(command.UserId, null, ct);
    return Result.Success();
  }
}