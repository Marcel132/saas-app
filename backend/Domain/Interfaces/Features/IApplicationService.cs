using backend.Api.Controllers.Applications.DTOs;

namespace backend.Domain.Interfaces.Features;

public interface IApplicationService
{
  public Task AcceptApplicationAsync(Guid userId, long applicationId);
  public Task RejectApplicationAsync(Guid userId, long applicationId);
  public Task<List<ApplicationDto>> GetCurrentUserApplications(Guid userId);
}