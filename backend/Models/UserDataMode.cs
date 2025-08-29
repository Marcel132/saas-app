using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class UserDataModel
{
  [Column("user_id")]
  [Key]
  public int UserId { get; set; }

  [JsonIgnore]
  [ForeignKey("UserId")]
  public UsersModel? User { get; set; }

  [Column("first_name")]
  [Required]
  public string FirstName { get; set; } = string.Empty;

  [Column("last_name")]
  [Required]
  public string LastName { get; set; } = string.Empty;

  [Column("phone_number")]
  [Required]
  [Phone]
  public string PhoneNumber { get; set; } = string.Empty;

  [Column("city")]
  [Required]
  public string City { get; set; } = string.Empty;

  [Column("country")]
  [Required]
  public string Country { get; set; } = string.Empty;

  [Column("postal_code")]
  [Required]
  public string PostalCode { get; set; } = string.Empty;

  [Column("street")]
  [Required]
  public string Street { get; set; } = string.Empty;

  [Column("company_name")]
  public string? CompanyName { get; set; } = string.Empty;

  [Column("company_nip")]
  public string? CompanyNip { get; set; } = string.Empty;

  [Column("is_email_verified")]
  [Required]
  public bool IsEmailVerified { get; set; } = false;

  [Column("is_two_factor_enabled")]
  [Required]
  public bool IsTwoFactorEnabled { get; set; } = false;

  [Column("is_profile_completed")]
  [Required]
  public bool IsProfileCompleted { get; set; } = false;

}