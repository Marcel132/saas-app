using backend.Api.Controllers.Contracts.DTOs;
using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Contracts.Queries;

public sealed class GetContractByIdQueryHandler : IQueryHandler<GetContractByIdQuery, ContractDetailsDto>
{
  private readonly IContractQueryRepository _repo;
  public GetContractByIdQueryHandler(
    IContractQueryRepository contractQueryRepository
  )
  {
    _repo = contractQueryRepository;
  }

  public async Task<Result<ContractDetailsDto>> HandleAsync(GetContractByIdQuery query, CancellationToken ct)
  {
    if (query.ContractId <= 0 || (query.UserId is not null && query.UserId == Guid.Empty))
      return Result<ContractDetailsDto>.Failure(new Error(
        DomainCodes.Validation.InvalidValue,
        "Błędne dane wejściowe",
        HttpResponseState.BadRequest
      ));

    var contract = await _repo.GetContractDetailsAsync(query.ContractId, query.UserId, ct);

    if (contract is null)
      return Result<ContractDetailsDto>.Failure(new Error(
        DomainCodes.General.NotFound,
        "Nie znaleziono",
        HttpResponseState.NotFound
      ));

    return contract;
  }
}