using backend.Application.Features.Auth.Dto;
using backend.Application.Services;
using backend.Domain.Interfaces.Features;

namespace backend.Application.Features.Auth.Shared;

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

  public async Task<CredentialsDto> GenerateCredentials(Guid userId, CancellationToken ct)
  {
    var permissions = await _roleService.GetEffectivePermissions(userId, ct);

    var authToken = _tokenService.GenerateAuthToken(userId, permissions);
    var refreshToken = _tokenService.GenerateRefreshToken();

    return new CredentialsDto(
      authToken,
      refreshToken
    );
  }

}