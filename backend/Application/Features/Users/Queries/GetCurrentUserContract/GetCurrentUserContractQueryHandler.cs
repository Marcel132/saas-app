using backend.Api.Controllers.Users.DTOs;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Users.Queries;

public sealed class GetCurrentUserContractQueryHandler : IQueryHandler<GetCurrentUserContractQuery, List<UserContractsDto>>
{
  private readonly IUserQueryRepository _repo;

  public GetCurrentUserContractQueryHandler(
    IUserQueryRepository userQueryRepository
  )
  {
    _repo = userQueryRepository;
  }

  public async Task<Result<List<UserContractsDto>>> HandleAsync(GetCurrentUserContractQuery query, CancellationToken ct)
  {
    return await _repo.GetCurrentUserContractsAsync(query.UserId, query.Status, ct);
  }
}