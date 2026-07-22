using backend.Api.Controllers.Users.DTOs;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Entities.Enum;

namespace backend.Application.Features.Users.Queries;

public sealed record GetCurrentUserContractQuery(
  Guid UserId,
  ContractStatus? Status
) : IQuery<List<UserContractsDto>>;