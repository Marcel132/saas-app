using backend.Application.Services.Auth.DTOs;

namespace backend.Domain.Interfaces.Services;

public interface ICredentialsService
{
  public Task<CredentialsDto> GenerateCredentials(Guid userId);
}