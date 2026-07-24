using backend.Api.Controllers;
using backend.Api.Controllers.Contracts.DTOs;
using backend.Application.Abstractions.CQRS;

namespace backend.Application.Features.Contracts.Queries;

public sealed record GetPublicContractsQuery(
  QueryParams QueryParams
) : IQuery<PagedResponse<PublicContractDto>>;