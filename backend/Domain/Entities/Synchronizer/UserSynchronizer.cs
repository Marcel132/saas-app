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
      new[] {Roles.USER.ToString(), Roles.DEVELOPER.ToString(), Roles.COMPANY.ToString() }
    );

    EnsureRole(user, roles[Roles.USER.ToString()].RoleId);

    if (user.IsCompany)
    {
      EnsureRole(user, roles[Roles.COMPANY.ToString()].RoleId);
      RemoveRole(user, roles[Roles.DEVELOPER.ToString()].RoleId);
    }
    else
    {
      EnsureRole(user, roles[Roles.DEVELOPER.ToString()].RoleId);
      RemoveRole(user, roles[Roles.COMPANY.ToString()].RoleId);
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