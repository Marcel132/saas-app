using backend.Application.Services.Auth.DTOs;

namespace backend.Domain.Interfaces.Features;

public interface ICredentialsService
{
  public Task<CredentialsDto> GenerateCredentials(Guid userId);
}