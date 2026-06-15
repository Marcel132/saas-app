public interface IAuthService
{
  public Task<CredentialsDto> LoginAsync(LoginRequestDto request, string ipAddress, string userAgent);

  public Task<CredentialsDto> RegisterAsync(RegisterRequestDto request, string ipAddress, string userAgent);

  public Task LogoutAsync(Guid userId);

  public Task<CredentialsDto> RefreshTokenAsync(string ipAddress, string userAgent, string? refreshToken);
}