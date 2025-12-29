using Microsoft.AspNetCore.Authorization;

public class HasPermissionAttribute : AuthorizeAttribute
{
  public HasPermissionAttribute(string permission)
  {
    Policy = permission;
  }
}