using backend.Api.Controllers.Contracts.DTOs;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Contracts.Queries;

public sealed class GetContractApplicationsQueryHandler : IQueryHandler<GetContractApplicationsQuery, List<ContractApplicationsDto>>
{
  private readonly IContractQueryRepository _repoQuery;
  private readonly IContractRepository _repo;

  public GetContractApplicationsQueryHandler(
    IContractQueryRepository contractQueryRepository,
    IContractRepository contractRepository
  )
  {
    _repoQuery = contractQueryRepository;
    _repo = contractRepository;
  }

  public async Task<Result<List<ContractApplicationsDto>>> HandleAsync(GetContractApplicationsQuery query, CancellationToken ct)
  {
    if (query.UserId == Guid.Empty || query.ContractId <= 0)
      return Result<List<ContractApplicationsDto>>.Failure(new Error(
        DomainCodes.Validation.InvalidValue,
        "Niepoprawne dane wejściowe",
        HttpResponseState.BadRequest
      ));

    var contract = await _repo.GetContractByIdAsync(query.ContractId, ct);

    if (contract is null)
      return Result<List<ContractApplicationsDto>>.Failure(new Error(
        DomainCodes.Contract.NotFound,
        "Nie znaleziono kontraktu",
        HttpResponseState.NotFound
      ));

    if (contract.AuthorId != query.UserId)
      return Result<List<ContractApplicationsDto>>.Failure(new Error(
        DomainCodes.Auth.Forbidden,
        "Nie masz uprawnień do zarządzania kontraktem",
        HttpResponseState.Forbidden
      ));

    return await _repoQuery.GetContractApplicationsAsync(query.ContractId, ct);
  }
}