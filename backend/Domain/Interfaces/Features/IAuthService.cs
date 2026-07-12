namespace backend.Domain.Interfaces.Features;

public interface IAuthService
{
  public Task LogoutAsync(Guid userId);

  public Task<CredentialsDto> RefreshTokenAsync(string ipAddress, string userAgent, string? refreshToken);
}