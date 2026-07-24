using backend.Api.Http;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Entities.Enum;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Users.Queries;

public sealed class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, object>
{
  private readonly IUserQueryRepository _repo;
  public GetCurrentUserQueryHandler(
    IUserQueryRepository userQueryRepository
  )
  {
    _repo = userQueryRepository;
  }

  public async Task<Result<object>> HandleAsync(GetCurrentUserQuery query, CancellationToken ct)
  {
    var roleType = await _repo.GetRoleTypeAsync(query.UserId, ct);

    return roleType switch
    {
      RoleType.Company => (object)await _repo.GetCurrentCompanyAsync(query.UserId, ct),
      RoleType.Pentester => (object)await _repo.GetCurrentPentesterAsync(query.UserId, ct),
      _ => Result.Failure(new Error(
        DomainCodes.General.BadRequest,
        "Nie znaleziono roli",
        HttpResponseState.BadRequest
      ))
    };
  }

}