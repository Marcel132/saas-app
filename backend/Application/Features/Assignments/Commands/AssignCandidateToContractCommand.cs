using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Assignments.Commands;

public sealed record AssignCandidateToContractCommand(  
  Guid UserId,
  long ContractId,
  Guid DeveloperId
) : ICommand;