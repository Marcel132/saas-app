using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces.Features;

namespace backend.Application.Features.Auth.Commands;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
  private readonly IAuthSessionService _sessionService;
  public LogoutCommandHandler(
    IAuthSessionService authSessionService
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