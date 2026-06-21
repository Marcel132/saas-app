using backend.Domain.EntitiesNew.Enum;
using backend.Domain.Interfaces.Repositories;

namespace backend.Domain.EntitiesNew;

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
      new[] { Roles.BASE_USER.ToString(), Roles.PENTESTER.ToString(), Roles.COMPANY.ToString() }
    );

    EnsureRole(user, roles[Roles.BASE_USER.ToString()].RoleId);

    if (user.RoleType == RoleType.Company)
    {
      EnsureRole(user, roles[Roles.COMPANY.ToString()].RoleId);
      RemoveRole(user, roles[Roles.PENTESTER.ToString()].RoleId);
    }
    if(user.RoleType == RoleType.Pentester)
    {
      EnsureRole(user, roles[Roles.PENTESTER.ToString()].RoleId);
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