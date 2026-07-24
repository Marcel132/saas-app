using backend.Api.Controllers;
using backend.Api.Controllers.Contracts.DTOs;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Application.Validators;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Contracts.Queries;

public sealed class GetCompanyContractsQueryHandler : IQueryHandler<GetCompanyContractsQuery, PagedResponse<CompanyContractDto>>
{
  private readonly IContractQueryRepository _repo;
  public GetCompanyContractsQueryHandler(
    IContractQueryRepository contractQueryRepository
  )
  {
    _repo = contractQueryRepository;
  }

  public async Task<Result<PagedResponse<CompanyContractDto>>> HandleAsync(GetCompanyContractsQuery query, CancellationToken ct)
  {
    var validationResult = QueryParamsValidator.Validate(query.QueryParams, query.UserId);

    if(validationResult.IsFailure)
      return Result<PagedResponse<CompanyContractDto>>.Failure(validationResult.Error);

    return await _repo.GetCompanyContractsAsync(query.UserId, query.QueryParams, ct);
  }
}