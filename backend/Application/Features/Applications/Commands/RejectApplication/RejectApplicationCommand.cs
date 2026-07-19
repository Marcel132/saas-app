using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Applications.Commands;

public sealed record RejectApplicationCommand(
  Guid UserId,
  long ApplicationId
) : ICommand;