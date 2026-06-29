using backend.Domain.Entities.Records;

namespace backend.Domain.Entities;

public class Permission
{
  public Guid Id { get; private set; }
  public string Resource { get; private set; } = null!;
  public string Action { get; private set; } = null!;
  public string Code { get; private set; } = null!;
  public string Description { get; private set; } = null!;
  public bool IsActive { get; private set; }
  public DateTime CreatedAt { get; private set; }

  private Permission() { }

  public Permission(PermissionRecord data)
  {
    data = NormalizeArguments(data);
    ValidateRequiredFields(data);
    ValidatePermission(data);

    Id = Guid.NewGuid();
    Action = data.Action;
    Resource = data.Resource;
    Code = $"{Resource}.{Action}";
    Description = data.Description;
    IsActive = true;
    CreatedAt = DateTime.UtcNow;
  }

  public void Deactivate()
  {
    if (!IsActive)
      throw new InvalidOperationAppException("Permission is deactivated");

    IsActive = false;
  }
  public void Activate()
  {
    if (IsActive)
      throw new InvalidOperationAppException("Permission is active");
    IsActive = true;
  }

  private static void ValidateRequiredFields(PermissionRecord data)
  {
    if (string.IsNullOrWhiteSpace(data.Action))
      throw new BadRequestAppException("Action is invalid.");
    if (string.IsNullOrWhiteSpace(data.Resource))
      throw new BadRequestAppException("Resource is invalid.");
    if (string.IsNullOrWhiteSpace(data.Description))
      throw new BadRequestAppException("Description is invalid.");
  }

  private static PermissionRecord NormalizeArguments(PermissionRecord data)
  {
    return data with
    {
      Action = data.Action.Trim().ToLowerInvariant(),
      Resource = data.Resource.Trim().ToLowerInvariant(),
      Description = data.Description.Trim()
    };
  }

  private static void ValidatePermission(PermissionRecord data)
  {
    if (data.Action.Length > 50 || data.Resource.Length > 50 || data.Description.Length > 200)
    {
      throw new BadRequestAppException("One or more parameters exceed maximum length.");
    }

    if (!IsValidIdentifier(data.Action) || !IsValidIdentifier(data.Resource))
    {
      throw new BadRequestAppException("One or more parameters are invalid.");
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