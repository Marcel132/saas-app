public class Permission
{
  public Guid PermissionId { get; private set; }
  public string Action { get; private set; } = null!;
  public string Resource { get; private set; } = null!;
  public string Code { get; private set; } = null!;
  public string Description { get; private set; } = null!;
  public bool IsActive { get; private set; }
  public DateTime CreatedAt { get; private set; }

  Permission() {} //EF Core

  public Permission(string action, string resource, string description)
  {
    ValidateRequiredFields(action, resource, description);
    (action, resource, description) = NormalizeArguments(action, resource, description);
    ValidatePermission(action, resource, description);

    PermissionId = Guid.NewGuid();
    Action = action;
    Resource = resource;
    Code = $"{Action}.{Resource}";
    Description = description;
    IsActive = true;
    CreatedAt = DateTime.UtcNow;
  }

  public void Deactivate()
  {
    IsActive = false;
  }
  public void Activate()
  {
    IsActive = true;
  }

  private static void ValidateRequiredFields(string action, string resource, string description)
  {
    if(string.IsNullOrWhiteSpace(action))
      throw new ArgumentException("Action is invalid.");
    if(string.IsNullOrWhiteSpace(resource))
      throw new ArgumentException("Resource is invalid.");
    if(string.IsNullOrWhiteSpace(description))
      throw new ArgumentException("Description is invalid.");
  }

  private static
  (string action, string resource, string description)
   NormalizeArguments(string action, string resource, string description)
  {
    return (
      action.Trim().ToLowerInvariant(),
      resource.Trim().ToLowerInvariant(),
      description.Trim()
    );
  }
  
  private static void ValidatePermission(string action, string resource, string description)
  {
    if(action.Length > 50 || resource.Length > 50 || description.Length > 200)
    {
      throw new ArgumentException("One or more parameters exceed maximum length.");
    }

    if(!IsValidIdentifier(action) || !IsValidIdentifier(resource))
    {
      throw new ArgumentException("One or more parameters are invalid.");
    }
  }
  private static bool IsValidIdentifier(string str)
  {
    return str.All(c =>
      char.IsLetterOrDigit(c)
      || c == '_'
      || c == '-');
  }
}