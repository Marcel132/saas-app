using backend.Api.Controllers;
using backend.Api.Controllers.Contracts.DTOs;
using backend.Application.Abstractions.CQRS;
using backend.Application.Validators;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Contracts.Queries;

public sealed class GetOpenContractsQueryHandler : IQueryHandler<GetOpenContractsQuery, PagedResponse<OpenContractDto>>
{
  private readonly IContractQueryRepository _repo;
  public GetOpenContractsQueryHandler(
    IContractQueryRepository contractQueryRepository
  )
  {
    _repo = contractQueryRepository;
  }

  public async Task<Result<PagedResponse<OpenContractDto>>> HandleAsync(GetOpenContractsQuery query, CancellationToken ct)
  {
    var validationResult = QueryParamsValidator.Validate(query.QueryParams, query.UserId);

    if(validationResult.IsFailure)
      return Result<PagedResponse<OpenContractDto>>.Failure(validationResult.Error);

    return await _repo.GetOpenContractsAsync(query.UserId, query.QueryParams, ct);
  }
}