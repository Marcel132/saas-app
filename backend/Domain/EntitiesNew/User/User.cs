using backend.Domain.Entities.Records;
using backend.Domain.EntitiesNew.Enum;

namespace backend.Domain.EntitiesNew;

public class User
{
  public Guid Id { get; private set; }
  public string Email { get; private set; } = string.Empty;
  public string PasswordHash { get; private set; } = string.Empty;
  public bool IsActive { get; private set; }
  public RoleType RoleType { get; private set; }
  public DateTime CreatedAt { get; private set; }

  public int FailedLoginAttempts { get; private set; }
  public DateTime? LoginBlockedUntil { get; private set; }

  private User() { } // EF

  public CompanyProfile? CompanyProfile { get; private set; }
  public PentesterProfile? PentesterProfile { get; private set; }

  private readonly List<UserRole> _userRoles = [];
  private readonly List<UserPermission> _userPermissions = [];

  public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();
  public IReadOnlyCollection<UserPermission> UserPermissions => _userPermissions.AsReadOnly();


  public User(UserRecord data)
  {
    Id = Guid.NewGuid();
    Email = data.NormalizedEmail;
    PasswordHash = data.PasswordHash;
    RoleType = data.Role;
    IsActive = true;
    CreatedAt = DateTime.UtcNow;
  }

  public void CreateCompanyProfile(CompanyProfileRecord data)
  {
    if (RoleType != RoleType.Company)
      throw new InvalidOperationAppException();
    
    if(CompanyProfile is not null)
      throw new InvalidOperationAppException();

    CompanyProfile = new CompanyProfile(Id,data);
  }

  public void CreatePentesterProfile(PentesterProfileRecord data)
  {
    if (RoleType != RoleType.Pentester)
      throw new InvalidOperationAppException();
    
    if(PentesterProfile is not null)
      throw new InvalidOperationAppException();

    PentesterProfile = new PentesterProfile(Id, data);
  }

  public bool CanLogin()
  {
    if (LoginBlockedUntil != null && LoginBlockedUntil <= DateTime.UtcNow)
    {
      ResetFailedLoginAttempts();
    }
    return LoginBlockedUntil == null
      || LoginBlockedUntil <= DateTime.UtcNow;
  }
  public void RegisterFailedLoginAttempt(int maxAttempts, TimeSpan blockDuration)
  {
    FailedLoginAttempts++;

    if (FailedLoginAttempts >= maxAttempts)
    {
      LoginBlockedUntil = DateTime.UtcNow.Add(blockDuration);
    }
  }
  public void ResetFailedLoginAttempts()
  {
    FailedLoginAttempts = 0;
    LoginBlockedUntil = null;
  }

  public void DeactivateAccount()
  {

    if (!IsActive)
      return;

    IsActive = false;
  }

  public void DeleteAccount()
  {
    DeactivateAccount();
    Email = $"deleted_{Id}@local";

    CompanyProfile?.Anonymize();
    PentesterProfile?.Anonymize();
  }

  public void AddRole(Guid roleId)
  {
    if (_userRoles.Any(x => x.RoleId == roleId))
      return;

    _userRoles.Add(new UserRole(Id, roleId));
  }
  public void RemoveRole(Guid roleId)
  {
    _userRoles.RemoveAll(x => x.RoleId == roleId);
  }
  public void AddPermission(Guid permissionId)
  {
    if (_userPermissions.Any(x => x.PermissionId == permissionId))
      return;

    _userPermissions.Add(
        new UserPermission(Id, permissionId)
    );
  }
  public void RemovePermission(Guid permissionId)
  {
    _userPermissions.RemoveAll(
        x => x.PermissionId == permissionId
    );
  }
}