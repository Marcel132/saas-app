using System.ComponentModel.DataAnnotations;

public class UserModel
{
  public int Id { get; set; }

  [EmailAddress]
  public string Email { get; set; } = string.Empty;
  [Required]
  public string PasswordHash { get; set; } = string.Empty;
  public enum RoleType { Admin, Company, Pentester}
  public RoleType Role { get; set; }

  // This properties are optional and can be used for pentesters or companies  
  public string? FirstName { get; set; }
  public string? LastName { get; set; }

  [Phone]
  public string? PhoneNumber { get; set; }

  public CompanyInfo? Company { get; set; }
  public AddressInfo? Address { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public bool IsActive { get; set; } = true;

  // For admin
  public string? LastLoginIp { get; set; }
  public DateTime? LastLoginAt { get; set; }
  public bool IsEmailConfirmed { get; set; } = false;
  public bool IsTwoFactorEnabled { get; set; } = false;
  public bool IsAccountCompleted { get; set; } = false;
}

public class CompanyInfo
{
    public string CompanyName { get; set; } = string.Empty;
    public string Nip { get; set; } = string.Empty;
}

public class AddressInfo
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}