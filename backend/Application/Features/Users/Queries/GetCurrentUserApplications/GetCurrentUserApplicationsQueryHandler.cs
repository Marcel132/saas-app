using backend.Api.Controllers.Users.DTOs;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Users.Queries;

public sealed class GetCurrentUserApplicationsQueryHandler : IQueryHandler<GetCurrentUserApplicationsQuery, List<UserApplicationsDto>>
{
  private readonly IUserQueryRepository _repo;
  public GetCurrentUserApplicationsQueryHandler(
    IUserQueryRepository userQueryRepository
  )
  {
    _repo = userQueryRepository;
  }

  public async Task<Result<List<UserApplicationsDto>>> HandleAsync(GetCurrentUserApplicationsQuery query, CancellationToken ct)
  {
    return await _repo.GetApplicationsAsync(query.UserId, query.Status, ct);
  }
}