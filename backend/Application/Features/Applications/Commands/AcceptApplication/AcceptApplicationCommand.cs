using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Applications.Commands;

public sealed record AcceptApplicationCommand(
  Guid UserId,
  long ApplicationId
) : ICommand;