using Microsoft.AspNetCore.Authorization;

namespace backend.Api.Auth;

public class PermissionRequirement : IAuthorizationRequirement
{
  public string Permission { get; }

  public PermissionRequirement(string permission)
  {
    Permission = permission;
  }
}