using System.Text.RegularExpressions;

public class User
{
  private static readonly Regex EmailRegex = new(
    @"^(?=.{1,254}$)(?=.{1,64}@)([A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*)@([A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?)(\.[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?)+$",
    RegexOptions.Compiled | RegexOptions.CultureInvariant);
  private static readonly Regex PasswordRegex = new(
    @"^(?=.{8,128}$)(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9])\S+$",
    RegexOptions.Compiled | RegexOptions.CultureInvariant);
  private static readonly Regex BcryptHashRegex = new(
    @"^\$2[abxy]\$\d{2}\$[./A-Za-z0-9]{53}$",
    RegexOptions.Compiled | RegexOptions.CultureInvariant);

  private readonly List<UserSpecialization> _userSpecializations = new();
  private readonly List<UserRole> _userRole = new();
  public IReadOnlyCollection<UserSpecialization> UserSpecializations => _userSpecializations.AsReadOnly();
  public IReadOnlyCollection<UserRole> UserRoles => _userRole.AsReadOnly();

  public IReadOnlyCollection<Specialization> Specializations
    => _userSpecializations.Select(s => s.Specialization).ToList();
    
  public Guid Id { get; private set; }
  public string Email { get; private set; } = string.Empty;
  public string PasswordHash { get; private set; } = string.Empty;
  public bool IsActive { get; private set; }
  public DateTime CreatedAt { get; private set; }

  public int FailedLoginAttempts { get; private set; }
  public DateTime? LoginBlockedUntil { get; private set; }


  public UserData UserData { get; private set; } = null!;
  private User() { } // EF

  public User(string email, string passwordHash, UserData userData)
  {

    Id = Guid.NewGuid();
    Email = email.Trim().ToLowerInvariant();
    PasswordHash = passwordHash;
    IsActive = true;
    CreatedAt = DateTime.UtcNow;

    UserData = userData
      ?? throw new InvalidOperationException("UserData is required");
  }

  //  Domain behaviors

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


  public void AddSpecialization(Specialization specialization)
  {
    if (_userSpecializations.Any(s => s.Specialization == specialization))
      return;

    _userSpecializations.Add(new UserSpecialization(Id, specialization));
  }
  public void RemoveSpecialization(Specialization specialization)
  {
    var toRemove = _userSpecializations
      .FirstOrDefault(s => s.Specialization == specialization);

    if (toRemove != null)
      _userSpecializations.Remove(toRemove);
  }
  public void ClearSpecializations()
  {
    _userSpecializations.Clear();
  }


  public void SetUserData(UserData data)
  {
    UserData = data;
  }
  public void UpdateUserData(
    string? firstName,
    string? lastName,
    string? phoneNumber,
    string? skills,
    string? city,
    string? country,
    string? postalCode,
    string? street,
    string? companyName,
    string? companyNip
    )
    {
      if(UserData == null)
      {
        UserData = new UserData(
          firstName ?? string.Empty,
          lastName ?? string.Empty,
          phoneNumber ?? string.Empty,
          skills ?? string.Empty,
          city ?? string.Empty,
          country ?? string.Empty,
          postalCode ?? string.Empty,
          street ?? string.Empty,
          companyName,
          companyNip
        );

        return;
      }
      else
      {
        UserData.Update(
        firstName,
        lastName,
        phoneNumber,
        skills,
        city,
        country,
        postalCode,
        street,
        companyName,
        companyNip
      );
      }
    }

  public void DeactivateAccount()
  {
    IsActive = false;
  }
  public void AnonymizeEmail()
  {
    Email = $"deleted_{DateTime.UtcNow:yyyyMMddHHmmss}_{Id}@local";
  }
  
  // Validation 
  public static bool IsValidPasswordFormat(string password)
  {
    return password.Length >= 8
      && password.Length <= 128
      && password.Any(char.IsUpper)
      && password.Any(char.IsLower)
      && password.Any(char.IsDigit)
      && password.Any(c => !char.IsLetterOrDigit(c));
  }
  public static bool IsValidEmailFormat(string email)
  {
    return IsValidEmail(email);
  }

  // Validation helpers
  private static bool IsValidEmail(string email)
  {
    if (string.IsNullOrWhiteSpace(email))
      return false;

    if (email.Length > 254)
      return false;

    if (!string.Equals(email, email.Trim(), StringComparison.Ordinal))
      return false;

    return EmailRegex.IsMatch(email);
  }
  private static bool IsValidPasswordHash(string hash)
  {
    if (string.IsNullOrWhiteSpace(hash))
      return false;

    return BcryptHashRegex.IsMatch(hash);
  }

  public void ChangeEmail(string email)
  {
    if (!IsValidEmail(email))
      throw new InvalidFormatAppException();

    Email = email.Trim().ToLowerInvariant();
  }
  public void ChangePassword(string hash)
  {
    if (!IsValidPasswordHash(hash))
      throw new InvalidFormatAppException();

    PasswordHash = hash;
  }
  public void DeleteAccount()
  {
    DeactivateAccount();
    AnonymizeEmail();
    ClearSpecializations();
    UserData.ClearPersonalData();
    _userRole.Clear();
  }
  public bool IsCompany =>
    _userSpecializations.Any(s => s.Specialization == Specialization.Company);

  public void AddRole(Guid roleId)
  {
    if (_userRole.Any(r => r.RoleId == roleId))
      return;

    _userRole.Add(new UserRole(Id, roleId));
  }
  public void RemoveRole(Guid roleId)
  {
    _userRole.RemoveAll(r => r.RoleId == roleId);
  }
}
