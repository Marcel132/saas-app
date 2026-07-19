using backend.Api.Controllers.Applications.DTOs;

namespace backend.Domain.Interfaces.Repositories;
public interface IApplicationQueryRepository
{
  public Task<List<ApplicationDto>> GetUserApplications(Guid userId, CancellationToken ct);
}