public interface IUserRepository
{
  Task<bool> ExistsByEmailAsync(string email);
  Task<bool> ExistsByNicknameAsync(string nickname);
  Task<User?> GetByEmailAsync(string email);
  Task<User?> GetByIdAsync(Guid id);
  Task AddAsync(User user);
  Task UpdateAsync(User user);
  Task DeleteAsync(User user);
  Task SaveChangesAsync();
}