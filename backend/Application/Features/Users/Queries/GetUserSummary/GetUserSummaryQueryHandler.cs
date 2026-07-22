using backend.Api.Controllers.Users.DTOs;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Users.Queries;

public sealed class GetUserSummaryQueryHandler : IQueryHandler<GetUserSummaryQuery, UserSummaryDto>
{
  private readonly IUserQueryRepository _repo;
  public GetUserSummaryQueryHandler(
    IUserQueryRepository userQueryRepository
  )
  {
    _repo = userQueryRepository;
  }

  public async Task<Result<UserSummaryDto>> HandleAsync(GetUserSummaryQuery query, CancellationToken ct)
  {
    return await _repo.GetSummary(query.UserId, ct);
  }
}