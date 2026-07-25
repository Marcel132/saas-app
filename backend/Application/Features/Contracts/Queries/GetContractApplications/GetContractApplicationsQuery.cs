using backend.Api.Controllers.Contracts.DTOs;
using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Contracts.Queries;

public sealed record GetContractApplicationsQuery(
  Guid UserId,
  long ContractId
) : IQuery<List<ContractApplicationsDto>>;