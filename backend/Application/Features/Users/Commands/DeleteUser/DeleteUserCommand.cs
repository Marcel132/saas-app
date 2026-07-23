using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Users.Commands;

public sealed record DeleteUserCommand(
  Guid UserId
) : ICommand;