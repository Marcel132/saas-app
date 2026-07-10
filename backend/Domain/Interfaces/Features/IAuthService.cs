using backend.Api.Controllers.Auth.DTOs;
using backend.Application.Services.Auth.DTOs;

namespace backend.Domain.Interfaces.Features;

public interface IAuthService
{
  public Task<CredentialsDto> LoginAsync(LoginRequestDto request, string ipAddress, string userAgent);

  public Task<CredentialsDto> RegisterPentesterAsync(RegisterPentesterRequestDto request, string ipAddress, string userAgent);

  public Task<CredentialsDto> RegisterCompanyAsync(RegisterCompanyRequestDto request, string ipAddress, string userAgent);

  public Task LogoutAsync(Guid userId);

  public Task<CredentialsDto> RefreshTokenAsync(string ipAddress, string userAgent, string? refreshToken);
}