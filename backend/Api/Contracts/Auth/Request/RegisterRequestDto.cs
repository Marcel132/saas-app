using System.ComponentModel.DataAnnotations;

public class RegisterRequestDto
{
  [Required]
  [EmailAddress]
  [StringLength(254)]
  public string Email { get; set; } = string.Empty;

  [Required]
  [StringLength(128, MinimumLength = 8)]
  public string Password { get; set; } = string.Empty;
  
  [Required]
  [NoHtmlChars]
  public List<Specialization> SpecializationType { get; set; } = [];

  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(1028)]
  public string Skills { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(64)]
  public string FirstName { get; set; } = string.Empty;
  
  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(64)]
  public string LastName { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(32)]
  [Phone]
  public string PhoneNumber { get; set; } = string.Empty;
  
  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(128)]
  public string City { get; set; } = string.Empty;
  
  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(64)]
  public string Country { get; set; } = string.Empty;
  
  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(32)]
  public string PostalCode { get; set; } = string.Empty;
 
  [Required]
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(128)]
  public string Street { get; set; } = string.Empty;
  
  [StringLength(256)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? CompanyName { get; set; }
  
  [StringLength(128)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? CompanyNip { get; set; }
}