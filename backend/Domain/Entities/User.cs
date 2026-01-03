public class User
{
  private readonly List<string> _specializations = new();
  public Guid Id { get; private set; }
  public string Email { get; private set; } = string.Empty;
  public string PasswordHash { get; private set; } = string.Empty;
  public bool IsActive { get; private set; }
  public DateTime CreatedAt { get; private set; }

  public int FailedLoginAttempts { get; private set; }
  public DateTime? LoginBlockedUntil { get; private set; }

  public IReadOnlyCollection<string> Specializations => _specializations.AsReadOnly();

  public UserData? UserData { get; private set; }
  private User() { } // EF

  public User(string email, string passwordHash)
  {
    Id = Guid.NewGuid();
    Email = email;
    PasswordHash = passwordHash;
    IsActive = true;
    CreatedAt = DateTime.UtcNow;
  }


  //  Domain behaviors

  public bool CanLogin()
  {
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

  public void AddSpecialization(string specializaion)
  {
    if (string.IsNullOrWhiteSpace(specializaion))
      throw new ArgumentException("Invalid specialization");

    if (_specializations.Contains(specializaion))
      return;

    _specializations.Add(specializaion);
  }

  public void RemoveSpecialization(string specializaion)
  {
    _specializations.Remove(specializaion);
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
  string? companyNip)
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

  public void Deactivate()
  {
    IsActive = false;
  }

  public void ChangeEmail(string email)
  {
    Email = email;
  }

  public void ChangePassword(string hash)
  {
    PasswordHash = hash;
  }

  public void ClearSpecializations()
  {
    _specializations.Clear();
  }

}
