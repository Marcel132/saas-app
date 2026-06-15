public class CredentialsService : ICredentialsService
{
  private readonly TokenService _tokenService;
  private readonly RoleService _roleService;
  public CredentialsService(
    TokenService tokenService,
    RoleService roleService
  )
  {
    _tokenService = tokenService;
    _roleService = roleService;
  }

  public async Task<CredentialsDto> GenerateCredentials(Guid userId)
  {
    var permissions = await _roleService.GetEffectivePermissions(userId);

    var authToken = _tokenService.GenerateAuthToken(userId, permissions);
    var refreshToken = _tokenService.GenerateRefreshToken();

    return new CredentialsDto
    {
      AuthToken = authToken,
      RefreshToken = refreshToken
    };
  }

}