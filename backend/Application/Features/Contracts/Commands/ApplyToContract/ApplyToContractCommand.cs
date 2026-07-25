using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Contracts.Commands;

public sealed record ApplyToContractCommand(
  Guid CandidateId,
  long ContractId
) : ICommand;