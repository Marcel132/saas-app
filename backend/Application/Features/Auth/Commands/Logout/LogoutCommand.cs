using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Auth.Commands;

public sealed record LogoutCommand(
  Guid UserId
) : ICommand;