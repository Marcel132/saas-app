using backend.Domain.Entities;

namespace backend.Domain.Interfaces.Repositories;

/// <summary>
/// Repository for user persistance 
/// All data are normalized before comparisions
/// </summary>
public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
    Task<bool> ExistsByNicknameAsync(string nickname, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}