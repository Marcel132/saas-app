using backend.Domain.Interfaces.Repositories;

namespace backend.Application.Services;

public class RoleService
{
  private readonly IRoleRepository _roleRepository;
  public RoleService(
    IRoleRepository repository
  )
  {
    _roleRepository = repository;
  }

  public async Task<HashSet<string>> GetEffectivePermissions(Guid userId, CancellationToken ct)
  {
    return await _roleRepository.GetEffectivePermissionsAsync(userId, ct);
  }

}