public class UserRoleSynchronizer
{
  private readonly IRoleRepository _roleRepo;

  public UserRoleSynchronizer(IRoleRepository roleRepo)
  {
    _roleRepo = roleRepo;
  }

  public async Task SyncAsync(User user)
  {
    var roles = await _roleRepo.GetByCodesAsync(
      new[] {"USER", "DEVELOPER", "COMPANY", "ADMIN"}
    );

    EnsureRole(user, roles["USER"].RoleId);

    if (user.IsCompany)
    {
      EnsureRole(user, roles["COMPANY"].RoleId);
      RemoveRole(user, roles["DEVELOPER"].RoleId);
    }
    else
    {
      EnsureRole(user, roles["DEVELOPER"].RoleId);
      RemoveRole(user, roles["COMPANY"].RoleId);
    }
  }

  private void EnsureRole(User user, Guid roleId)
  {
    if (user.UserRoles.Any(r => r.RoleId == roleId))
      return;

    user.AddRole(roleId);
  }

  private void RemoveRole(User user, Guid roleId)
  {
    user.RemoveRole(roleId);
  }
}