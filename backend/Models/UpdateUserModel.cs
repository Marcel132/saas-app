using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UpdateUserModel
{
  [Column("uid")]
  [Required]
  public int Id { get; set; }

  [Column("password_hash")]
  public string PasswordHash { get; set; } = string.Empty;

  [Column("user_role")]
  public RoleEnum Role { get; set; }

  [Column("specialization")]
  public List<string> SpecializationType { get; set; } = new List<string>();

  [Column("skills")]
  public string Skills { get; set; } = string.Empty;

  [Column("is_active")]
  public bool IsActive { get; set; } = true;

  [Column("first_name")]
  public string FirstName { get; set; } = string.Empty;

  [Column("last_name")]
  public string LastName { get; set; } = string.Empty;

  [Column("phone_number")]
  [Phone]
  public string? PhoneNumber { get; set; }

  [Column("city")]
  public string City { get; set; } = string.Empty;

  [Column("country")]
  public string Country { get; set; } = string.Empty;

  [Column("postal_code")]
  public string PostalCode { get; set; } = string.Empty;

  [Column("street")]
  public string Street { get; set; } = string.Empty;

  [Column("company_name")]
  public string? CompanyName { get; set; }

  [Column("company_nip")]
  public string? CompanyNip { get; set; }

  [Column("is_email_verified")]
  public bool IsEmailVerified { get; set; } = false;

  [Column("is_two_factor_enabled")]
  public bool IsTwoFactorEnabled { get; set; } = false;

  [Column("is_profile_completed")]
  public bool IsProfileCompleted { get; set; } = false;

}