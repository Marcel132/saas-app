using backend.Api.Controllers.Applications.DTOs;
using backend.Application.Abstractions.CQRS;
using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Features.Applications.Queries;

public sealed class GetUserApplicationsQueryHandler : IQueryHandler<GetUserApplicationsQuery, List<ApplicationDto>>
{
  private readonly IApplicationQueryRepository _repo;
  public GetUserApplicationsQueryHandler(
    IApplicationQueryRepository applicationQueryRepository
  )
  {
    _repo = applicationQueryRepository;
  }

  public async Task<Result<List<ApplicationDto>>> HandleAsync(GetUserApplicationsQuery query, CancellationToken ct)
  {
    var applications = await _repo.GetUserApplications(query.UserId, ct);
    
    return Result<List<ApplicationDto>>.Success(applications);
  }
}