public interface IRoleRepository
{
  Task<Role> GetByCodeAsync(string code, CancellationToken ct = default);
  Task<IReadOnlyDictionary<string, Role>> GetByCodesAsync(
    IEnumerable<string> codes,
    CancellationToken ct = default
  );
}