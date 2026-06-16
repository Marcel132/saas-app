using backend.Domain.Entities;

namespace backend.Domain.Interfaces.Repositories;

public interface IRoleRepository
{
  Task<Role> GetByCodeAsync(string code, CancellationToken ct = default);

  Task<IReadOnlyDictionary<string, Role>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken ct = default);

  Task<HashSet<string>> GetEffectivePermissionsAsync(Guid userId);
}