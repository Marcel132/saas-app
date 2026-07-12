using backend.Application.Features.Auth.Dto;

namespace backend.Domain.Interfaces.Features;

public interface ICredentialsService
{
  public Task<CredentialsDto> GenerateCredentials(Guid userId);
}