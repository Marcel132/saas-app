using Microsoft.AspNetCore.Authorization;

namespace backend.Api.Auth;

public class HasPermissionAttribute : AuthorizeAttribute
{
  public HasPermissionAttribute(string permission)
  {
    Policy = permission;
  }
}