using backend.Api.Controllers;
using backend.Api.Controllers.Contracts.DTOs;
using backend.Application.Abstractions.CQRS;
using backend.Application.Validators;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Contracts.Queries;

public sealed class GetPublicContractsQueryHandler : IQueryHandler<GetPublicContractsQuery, PagedResponse<PublicContractDto>>
{
  private readonly IContractQueryRepository _repo;
  public GetPublicContractsQueryHandler(
    IContractQueryRepository contractQueryRepository
  )
  {
    _repo = contractQueryRepository;
  }

  public async Task<Result<PagedResponse<PublicContractDto>>> HandleAsync(GetPublicContractsQuery query, CancellationToken ct)
  {
    var validationResult  = QueryParamsValidator.Validate(query.QueryParams, null);
  
    if(validationResult .IsFailure)
      return Result<PagedResponse<PublicContractDto>>.Failure(validationResult.Error);

    return await _repo.GetPublicContractsAsync(query.QueryParams, ct);   
  }
}