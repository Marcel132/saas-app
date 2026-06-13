public interface IAuthService
{
  // TODO: delete HttpResponse
  public Task<AuthResult> LoginAsync(LoginRequestDto request, string ipAddress, string userAgent, HttpResponse response);

  public Task<AuthResult> RegisterAsync(RegisterRequestDto request, string ipAddress, string userAgent, HttpResponse response);

  public Task LogoutAsync(Guid userId, HttpResponse response);

  public Task<RefreshTokenResult> RefreshTokenAsync(string ipAddress, string userAgent, string? refreshToken);
}