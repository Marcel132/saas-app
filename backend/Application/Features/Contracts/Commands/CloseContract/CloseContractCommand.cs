using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Contracts.Commands;

public sealed record CloseContractCommand(
  Guid UserId,
  long ContractId
) : ICommand;