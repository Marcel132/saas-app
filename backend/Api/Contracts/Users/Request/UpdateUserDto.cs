using System.ComponentModel.DataAnnotations;

public class UpdateUserDto
{
  [EmailAddress]
  [StringLength(254)]
  public string? Email { get; set; }

  [StringLength((128), MinimumLength = 8)]
  public string? Password { get; set; }
  
  [NoHtmlChars]
  public List<string>? SpecializationType { get; set; }
  
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(1028)]
  public string? Skills { get; set; }
  
  [RegularExpression(@"^[^%<>]*$")]
  [StringLength(64)]
  public string? FirstName { get; set; }
  
  [StringLength(128)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? LastName { get; set; }
  
  [StringLength(32)]
  [Phone]
  public string? PhoneNumber { get; set; }
  
  [StringLength(128)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? City { get; set; }
  
  [StringLength(64)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? Country { get; set; }
  
  [StringLength(32)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? PostalCode { get; set; }

  [StringLength(128)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? Street { get; set; }
  
  [StringLength(256)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? CompanyName { get; set; }
  
  [StringLength(128)]
  [RegularExpression(@"^[^%<>]*$")]
  public string? CompanyNip { get; set; }
}