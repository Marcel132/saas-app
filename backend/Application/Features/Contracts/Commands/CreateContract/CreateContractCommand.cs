using backend.Api.Controllers.Contracts.DTOs;
using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Contracts.Commands;

public sealed record CreateContractCommand(
  Guid AuthorId,
  ContractRequestDto Request
) : ICommand;