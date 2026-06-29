using backend.Domain.Entities.Records;
using backend.Domain.Entities.Enum;

namespace backend.Domain.Entities;

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

  public void UpdatePentesterProfile(
    string? firstName,
    string? lastName,
    string? nickName,
    string? phone,
    string? country,
    string? city,
    string? street,
    string? postalCode,
    string? bio,
    string? githubUrl,
    string? linkedinUrl)
  {
    if (PentesterProfile is null)
      throw new InvalidOperationAppException();

    PentesterProfile.UpdateProfile(
      firstName, lastName, nickName, phone, country,
      city, street, postalCode, bio, githubUrl, linkedinUrl
    );
  }

  public void UpdateCompanyProfile(
    string? name,
    string? phone,
    string? country,
    string? city,
    string? street,
    string? postalCode,
    string? bio,
    string? websiteUrl)
  {
    if (CompanyProfile is null)
      throw new InvalidOperationAppException();

    CompanyProfile.UpdateProfile(
      name, phone, country, city, street, postalCode, bio, websiteUrl
    );
  }

  public void AddPentesterSpecialization(Specialization spec)
  {
    if (PentesterProfile is null)
      throw new InvalidOperationAppException();

    PentesterProfile.AddSpecialization(spec);
  }

  public void ClearPentesterSpecialization(Specialization spec)
  {
    if (PentesterProfile is null)
      throw new InvalidOperationAppException();

    PentesterProfile.ClearSpecialization(spec);
  }

  public void ClearAllPentesterSpecializations()
  {
    if (PentesterProfile is null)
      throw new InvalidOperationAppException();

    PentesterProfile.ClearAllSpecializations();
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